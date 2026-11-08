(function () {
    var root = document.documentElement;
    if (localStorage.getItem("janz-theme") === "day") root.classList.add("theme-day");
})();

var authOverlay = document.createElement("div");
authOverlay.className = "auth-overlay";
authOverlay.innerHTML =
    '<div class="auth-modal" role="dialog" aria-labelledby="auth-title" aria-modal="true">' +
        "<h2 id=\"auth-title\">Sign in</h2>" +
        '<form action="/Account/Login" method="post">' +
            '<p class="entry-error" hidden></p>' +
            '<label class="entry-field"><input type="email" name="email" placeholder="Email" autocomplete="email" required /></label>' +
            '<label class="entry-field"><input type="password" name="password" placeholder="Password" autocomplete="current-password" required /><button type="button" class="entry-eye" aria-label="Show password"><svg viewBox="0 0 24 24" aria-hidden="true"><path d="M3 12s3.6-6.5 9-6.5S21 12 21 12s-3.6 6.5-9 6.5S3 12 3 12z" fill="none" stroke="currentColor" stroke-width="1.6"/><circle cx="12" cy="12" r="2.4" fill="none" stroke="currentColor" stroke-width="1.6"/></svg></button></label>' +
            '<button type="submit" class="account-signin">Sign in</button>' +
            '<a class="entry-forgot" href="/Account/ForgotPassword">Forgot password?</a>' +
            '<label class="entry-remember"><input type="checkbox" name="remember" /> Remember password</label>' +
        "</form>" +
    "</div>";

function isSignedIn() {
    return document.documentElement.getAttribute("data-auth") === "1";
}

function needsAuth(target) {
    return target.closest(
        'a[href="/Wishlist"], [data-wish], [aria-label="Likes"], [aria-label="Save"], [data-article-like], [data-article-save], .article-comment-votes button, [data-cart-add]'
    );
}
document.body.appendChild(authOverlay);

function openAuth() {
    authOverlay.classList.add("is-on");
    document.querySelectorAll(".click-menu[open]").forEach(function (menu) {
        menu.removeAttribute("open");
    });
}

function closeAuth() {
    authOverlay.classList.remove("is-on");
}

authOverlay.addEventListener("click", function (event) {
    if (event.target === authOverlay) closeAuth();
});

document.addEventListener("keydown", function (event) {
    if (event.key !== "Escape") return;
    closeAuth();
    document.querySelectorAll(".search[open], .search-where[open]").forEach(function (menu) {
        menu.removeAttribute("open");
    });
});

authOverlay.querySelector("form").addEventListener("submit", function (event) {
    event.preventDefault();
    var form = event.currentTarget;
    var errorBox = form.querySelector(".entry-error");
    var body = new URLSearchParams();
    body.set("email", form.email.value);
    body.set("password", form.password.value);
    body.set("remember", form.remember.checked ? "true" : "false");
    body.set("__RequestVerificationToken", document.documentElement.getAttribute("data-csrf") || "");
    errorBox.hidden = true;
    fetch("/Account/Login", {
        method: "POST",
        headers: {
            "Content-Type": "application/x-www-form-urlencoded",
            "X-Requested-With": "XMLHttpRequest",
            "RequestVerificationToken": document.documentElement.getAttribute("data-csrf") || ""
        },
        body: body.toString()
    }).then(function (response) {
        if (response.ok) {
            window.location.reload();
            return;
        }
        return response.json().then(function (payload) {
            errorBox.textContent = (payload.errors || []).join(" ") || "Email or password is wrong.";
            errorBox.hidden = false;
        }).catch(function () {
            errorBox.textContent = "Email or password is wrong.";
            errorBox.hidden = false;
        });
    }).catch(function () {
        errorBox.textContent = "The API is not running.";
        errorBox.hidden = false;
    });
});

authOverlay.querySelectorAll(".entry-eye").forEach(function (button) {
    button.addEventListener("click", function () {
        var field = button.previousElementSibling;
        var hidden = field.type === "password";
        field.type = hidden ? "text" : "password";
        button.setAttribute("aria-label", hidden ? "Hide password" : "Show password");
    });
});

document.addEventListener("click", function (event) {
    var themeBtn = event.target.closest(".theme-toggle");
    if (themeBtn) {
        var day = document.documentElement.classList.toggle("theme-day");
        localStorage.setItem("janz-theme", day ? "day" : "night");
        themeBtn.setAttribute("aria-label", day ? "Night mode" : "Day mode");
        return;
    }
    var signLink = event.target.closest('a[href="/Account/Login"]');
    if (signLink) {
        event.preventDefault();
        openAuth();
        return;
    }
    if (!isSignedIn()) {
        var gated = needsAuth(event.target);
        if (gated) {
            event.preventDefault();
            openAuth();
            return;
        }
    }
    if (event.target.closest(".menu-toggle")) event.preventDefault();
    document.querySelectorAll(".click-menu[open]").forEach(function (menu) {
        if (!menu.contains(event.target)) menu.removeAttribute("open");
    });
    document.querySelectorAll(".search-where[open]").forEach(function (menu) {
        if (!menu.contains(event.target)) menu.removeAttribute("open");
    });
});

document.addEventListener("click", function (event) {
    var button = event.target.closest("[data-cart-add]");
    if (!button || !isSignedIn() || button.disabled) return;
    var body = new URLSearchParams();
    body.set("productId", button.getAttribute("data-cart-add"));
    body.set("quantity", "1");
    if (button.getAttribute("data-variant")) body.set("variantId", button.getAttribute("data-variant"));
    if (button.getAttribute("data-grind")) body.set("grind", button.getAttribute("data-grind"));
    body.set("__RequestVerificationToken", document.documentElement.getAttribute("data-csrf") || "");
    button.disabled = true;
    fetch("/Cart/Add", {
        method: "POST",
        headers: {
            "Content-Type": "application/x-www-form-urlencoded",
            "X-Requested-With": "XMLHttpRequest"
        },
        body: body.toString()
    }).then(function (response) {
        if (response.status === 401) {
            document.documentElement.setAttribute("data-auth", "0");
            openAuth();
            return;
        }
        return response.json().then(function (payload) {
            if (!response.ok) {
                alert((payload.errors || []).join(" ") || "Could not add to cart.");
                return;
            }
            setCartCount(payload.count);
            refreshMiniCart();
            showToast("Added to cart");
        });
    }).catch(function () {
        alert("The API is not running.");
    }).finally(function () {
        button.disabled = false;
    });
});

document.addEventListener("click", function (event) {
    var button = event.target.closest("[data-wish]");
    if (!button || !isSignedIn() || button.disabled) return;
    var add = !button.classList.contains("is-on");
    var body = new URLSearchParams();
    body.set("productId", button.getAttribute("data-wish"));
    body.set("add", add ? "true" : "false");
    body.set("__RequestVerificationToken", document.documentElement.getAttribute("data-csrf") || "");
    button.disabled = true;
    fetch("/Wishlist/Toggle", {
        method: "POST",
        headers: {
            "Content-Type": "application/x-www-form-urlencoded",
            "X-Requested-With": "XMLHttpRequest"
        },
        body: body.toString()
    }).then(function (response) {
        if (response.status === 401) {
            document.documentElement.setAttribute("data-auth", "0");
            openAuth();
            return;
        }
        if (!response.ok) {
            showToast("Could not update favorites");
            return;
        }
        return response.json().then(function (payload) {
            button.classList.toggle("is-on", add);
            button.setAttribute("aria-label", add ? "Remove from wishlist" : "Add to wishlist");
            document.querySelectorAll("[data-wish-count]").forEach(function (badge) {
                badge.textContent = payload.count;
                badge.hidden = payload.count === 0;
            });
            showToast(add ? "Added to favorites" : "Removed from favorites");
        });
    }).catch(function () {
        showToast("The API is not running.");
    }).finally(function () {
        button.disabled = false;
    });
});

document.querySelectorAll("#register .entry-eye, #signin .entry-eye").forEach(function (button) {
    button.addEventListener("click", function () {
        var field = button.previousElementSibling;
        var hidden = field.type === "password";
        field.type = hidden ? "text" : "password";
        button.setAttribute("aria-label", hidden ? "Hide password" : "Show password");
    });
});

document.querySelectorAll("#register form, #reset form").forEach(function (registerForm) {
    registerForm.addEventListener("submit", function (event) {
        var password = this.querySelector("[name='password']");
        var repeat = this.querySelector("[name='password2'], [name='repeatPassword']");
        if (password.value !== repeat.value) {
            event.preventDefault();
            repeat.setCustomValidity("Passwords do not match");
            repeat.reportValidity();
        } else {
            repeat.setCustomValidity("");
        }
    });
    registerForm.addEventListener("input", function (event) {
        if (event.target.name === "repeatPassword" || event.target.name === "password2") event.target.setCustomValidity("");
    });
});

document.querySelectorAll(".search").forEach(function (search) {
    var form = search.querySelector(".search-panel");
    var whereLabel = search.querySelector(".search-where > summary");
    var input = search.querySelector("input[name='q']");
    search.addEventListener("toggle", function () {
        if (search.open && input) input.focus();
    });
    search.querySelectorAll(".search-where-list button").forEach(function (option) {
        option.addEventListener("click", function () {
            search.querySelectorAll(".search-where-list button").forEach(function (item) {
                item.classList.toggle("is-on", item === option);
            });
            form.setAttribute("data-where", option.getAttribute("data-where"));
            whereLabel.textContent = option.textContent;
            search.querySelector(".search-where").removeAttribute("open");
        });
    });
    var suggest = form.querySelector(".search-suggest");
    var timer = null;
    input.addEventListener("input", function () {
        clearTimeout(timer);
        var query = input.value.trim();
        if (query.length < 2 || form.getAttribute("data-where") !== "/Products") {
            suggest.hidden = true;
            return;
        }
        timer = setTimeout(function () {
            fetch("/Products/Suggest?q=" + encodeURIComponent(query)).then(function (response) {
                return response.text();
            }).then(function (html) {
                if (input.value.trim() !== query) return;
                suggest.innerHTML = html;
                suggest.hidden = html.trim() === "";
            }).catch(function () {
                suggest.hidden = true;
            });
        }, 250);
    });
    search.addEventListener("toggle", function () {
        if (!search.open) suggest.hidden = true;
    });
    form.addEventListener("submit", function (event) {
        event.preventDefault();
        var where = form.getAttribute("data-where") || "/Products";
        var query = input.value.trim();
        window.location.href = query ? where + "?q=" + encodeURIComponent(query) : where;
    });
});

document.querySelectorAll(".wish-tabs").forEach(function (tabs) {
    var buttons = tabs.querySelectorAll("button");
    var panes = document.querySelectorAll(".wish-pane");
    buttons.forEach(function (button) {
        button.addEventListener("click", function () {
            buttons.forEach(function (item) {
                var on = item === button;
                item.classList.toggle("is-on", on);
                item.setAttribute("aria-pressed", on ? "true" : "false");
            });
            panes.forEach(function (pane) {
                pane.hidden = pane.getAttribute("data-pane") !== button.getAttribute("data-pane");
            });
        });
    });
});

document.querySelectorAll(".shot, .catalog-tile").forEach(function (card) {
    var video = card.querySelector("video");
    if (!video) return;
    card.addEventListener("mouseenter", function () {
        card.classList.add("is-playing");
        var playing = video.play();
        if (playing) playing.catch(function () {});
    });
    card.addEventListener("mouseleave", function () {
        card.classList.remove("is-playing");
        video.pause();
        video.currentTime = 0;
    });
});

document.querySelectorAll(".catalog-tags").forEach(function (tags) {
    var more = tags.querySelector(":scope > button");
    if (!more) return;
    function fitTags() {
        if (tags.classList.contains("is-open")) return;
        tags.classList.remove("has-more");
        tags.classList.toggle("has-more", tags.scrollHeight > tags.clientHeight + 1);
    }
    more.addEventListener("click", function () {
        var open = tags.classList.toggle("is-open");
        more.setAttribute("aria-expanded", open ? "true" : "false");
    });
    tags.addEventListener("transitionend", function (event) {
        if (event.propertyName === "max-height" && !tags.classList.contains("is-open")) fitTags();
    });
    fitTags();
    window.addEventListener("resize", fitTags);
});

function bindGrind(root) {
root.querySelectorAll(".grind").forEach(function (menu) {
    var label = menu.querySelector("summary span");
    menu.querySelectorAll(".grind-list button").forEach(function (option) {
        option.addEventListener("click", function () {
            menu.querySelectorAll(".grind-list button").forEach(function (item) {
                item.classList.toggle("is-on", item === option);
            });
            label.textContent = option.textContent;
            menu.removeAttribute("open");
            var buy = menu.closest("[data-product-buy]") || menu.closest(".product");
            var cart = buy && buy.querySelector("[data-cart-add]");
            if (cart) cart.setAttribute("data-grind", option.textContent);
        });
    });
});
}

bindGrind(document);

document.querySelectorAll(".gallery").forEach(function (gallery) {
    if (gallery.closest(".coffee-layout") && gallery.closest(".coffee-layout").querySelector("[data-variants]")) return;
    var photo = gallery.querySelector(".gallery-photo");
    var thumbs = gallery.querySelectorAll(".thumbs button");
    thumbs.forEach(function (thumb) {
        thumb.addEventListener("click", function () {
            thumbs.forEach(function (item) {
                item.classList.toggle("is-on", item === thumb);
            });
            photo.src = thumb.dataset.src;
        });
    });
});

document.querySelectorAll("[data-product-buy]").forEach(function (buy) {
    var source = buy.querySelector("[data-variants]");
    if (!source) return;
    var variants = JSON.parse(source.textContent);
    var page = buy.closest(".coffee-layout");
    var photo = page.querySelector(".gallery-photo");
    var thumbs = page.querySelector(".thumbs");
    var sizes = buy.querySelector("[data-sizes]");
    var grind = buy.querySelector(".grind");
    var stock = buy.querySelector("[data-variant-stock]");
    var cart = buy.querySelector("[data-cart-add]");
    var current = variants.filter(function (item) { return String(item.id) === cart.getAttribute("data-variant"); })[0];

    function money(value) {
        return "₼" + Number(value).toFixed(2);
    }

    function showGallery(format) {
        var list = variants.filter(function (item) { return item.format === format; });
        var images = [];
        list.forEach(function (item) { if (item.images[0] && images.indexOf(item.images[0]) === -1) images.push(item.images[0]); });
        list.forEach(function (item) {
            item.images.slice(1).forEach(function (image) { if (images.indexOf(image) === -1) images.push(image); });
        });
        if (!images.length) return;
        thumbs.innerHTML = "";
        images.forEach(function (image) {
            var button = document.createElement("button");
            button.type = "button";
            button.dataset.src = image;
            var img = document.createElement("img");
            img.src = image;
            img.alt = "";
            button.appendChild(img);
            thumbs.appendChild(button);
        });
    }

    function showPhoto(src) {
        photo.src = src;
        thumbs.querySelectorAll("button").forEach(function (item) {
            item.classList.toggle("is-on", item.dataset.src === src);
        });
    }

    function select(variant) {
        current = variant;
        cart.setAttribute("data-variant", variant.id);
        cart.disabled = variant.stock <= 0;
        cart.querySelector("strong").textContent = money(variant.price);
        cart.querySelector("small").hidden = !variant.discount;
        cart.querySelector("small").textContent = "-" + variant.discount + "%";
        cart.querySelector("s").hidden = !variant.discount;
        cart.querySelector("s").textContent = variant.oldPrice ? money(variant.oldPrice) : "";
        stock.textContent = variant.stock > 0 ? variant.stock + " in stock" : "Out of stock";
        grind.hidden = !variant.grind;
        if (variant.grind) {
            cart.setAttribute("data-grind", grind.querySelector("summary span").textContent);
        } else {
            cart.removeAttribute("data-grind");
        }
        sizes.querySelectorAll("button").forEach(function (item) {
            item.classList.toggle("is-on", item.getAttribute("data-variant-id") === String(variant.id));
        });
        if (variant.images[0]) showPhoto(variant.images[0]);
    }

    function cheapest(list) {
        var inStock = list.filter(function (item) { return item.stock > 0; });
        return (inStock.length ? inStock : list).slice().sort(function (a, b) { return a.price - b.price; })[0];
    }

    function showFormat(format) {
        buy.querySelectorAll("[data-format]").forEach(function (item) {
            item.classList.toggle("is-on", item.getAttribute("data-format") === format);
        });
        var list = variants.filter(function (item) { return item.format === format; });
        sizes.innerHTML = "";
        list.filter(function (item) { return item.size; }).forEach(function (item) {
            var button = document.createElement("button");
            button.type = "button";
            button.className = "weight";
            button.setAttribute("data-variant-id", item.id);
            button.textContent = item.size;
            if (item.discount) {
                var small = document.createElement("small");
                small.textContent = " -" + item.discount + "%";
                button.appendChild(small);
            }
            sizes.appendChild(button);
        });
        showGallery(format);
        select(cheapest(list));
    }

    buy.addEventListener("click", function (event) {
        var format = event.target.closest("[data-format]");
        if (format) {
            showFormat(format.getAttribute("data-format"));
            return;
        }
        var size = event.target.closest("[data-variant-id]");
        if (size) {
            select(variants.filter(function (item) { return String(item.id) === size.getAttribute("data-variant-id"); })[0]);
        }
    });

    thumbs.addEventListener("click", function (event) {
        var thumb = event.target.closest("button");
        if (thumb) showPhoto(thumb.dataset.src);
    });
});

document.addEventListener("click", function (event) {
    var weight = event.target.closest("[data-card-variant]");
    if (!weight) return;
    var card = weight.closest(".product");
    var cart = card.querySelector("[data-cart-add]");
    var discount = parseInt(weight.getAttribute("data-discount"), 10) || 0;
    card.querySelectorAll("[data-card-variant]").forEach(function (item) {
        item.classList.toggle("is-on", item === weight);
    });
    cart.setAttribute("data-variant", weight.getAttribute("data-card-variant"));
    cart.disabled = parseInt(weight.getAttribute("data-stock"), 10) <= 0;
    cart.querySelector("strong").textContent = "₼" + weight.getAttribute("data-price");
    cart.querySelector("small").hidden = !discount;
    cart.querySelector("small").textContent = "-" + discount + "%";
    cart.querySelector("s").hidden = !discount;
    cart.querySelector("s").textContent = discount ? "₼" + weight.getAttribute("data-old-price") : "";
    if (weight.getAttribute("data-image")) card.querySelector(".product-photo").src = weight.getAttribute("data-image");
});

function bindBuyRows(root) {
    root.querySelectorAll(".buy-row").forEach(function (row) {
        if (row.closest("[data-product-buy]") || row.querySelector("[data-card-variant]")) return;
        var weights = row.querySelectorAll(".weight");
        weights.forEach(function (weight) {
            weight.addEventListener("click", function () {
                weights.forEach(function (item) {
                    item.classList.toggle("is-on", item === weight);
                });
            });
        });
    });
}

bindBuyRows(document);

var channelsButton = document.querySelector("[data-channels]");
if (channelsButton) {
    channelsButton.addEventListener("click", function () {
        var panel = document.querySelector(".community-channels");
        panel.hidden = !panel.hidden;
        channelsButton.setAttribute("aria-pressed", panel.hidden ? "false" : "true");
    });
}

var promo = document.querySelector(".roast-card");
if (promo) {
    var track = promo.querySelector(".roast-track");
    var slides = promo.querySelectorAll(".roast-slide");
    var dots = promo.querySelectorAll(".dots i");
    var slideIndex = 0;
    function showSlide(next) {
        slideIndex = (next + slides.length) % slides.length;
        track.style.transform = "translateX(" + (-slideIndex * promo.clientWidth) + "px)";
        dots.forEach(function (dot, index) {
            dot.classList.toggle("is-on", index === slideIndex);
        });
    }
    promo.querySelector("[aria-label='Previous']").addEventListener("click", function () {
        showSlide(slideIndex - 1);
    });
    promo.querySelector("[aria-label='Next']").addEventListener("click", function () {
        showSlide(slideIndex + 1);
    });
}


var followOverlay = document.querySelector(".follow-overlay");
var followTitle = followOverlay ? followOverlay.querySelector("#follow-title") : null;
var followSearch = followOverlay ? followOverlay.querySelector(".follow-search input") : null;
var followCopy = {
    subscribers: { title: "My subscribers", search: "Search subscribers" },
    subscriptions: { title: "My subscriptions", search: "Search subscriptions" }
};

function openFollow(kind) {
    if (!followOverlay) return;
    var copy = followCopy[kind] || followCopy.subscribers;
    if (followTitle) followTitle.textContent = copy.title;
    if (followSearch) {
        followSearch.placeholder = copy.search;
        followSearch.value = "";
    }
    followOverlay.classList.add("is-on");
    var followList = followOverlay.querySelector(".follow-list");
    if (followList) {
        followList.innerHTML = "";
        fetch("/Community/People?kind=" + encodeURIComponent(kind), { headers: { "X-Requested-With": "XMLHttpRequest" } }).then(function (response) {
            return response.ok ? response.text() : "";
        }).then(function (html) {
            followList.innerHTML = html;
        }).catch(function () {});
    }
    if (followSearch) followSearch.focus();
}

function closeFollow() {
    if (followOverlay) followOverlay.classList.remove("is-on");
}

if (followOverlay) {
    followOverlay.addEventListener("click", function (event) {
        if (event.target === followOverlay) closeFollow();
    });
    if (followSearch) {
        followSearch.addEventListener("input", function () {
            var query = followSearch.value.trim().toLowerCase();
            followOverlay.querySelectorAll(".follow-item").forEach(function (item) {
                item.hidden = item.textContent.toLowerCase().indexOf(query) === -1;
            });
        });
    }
}

document.querySelectorAll("[data-follow]").forEach(function (button) {
    button.addEventListener("click", function () {
        openFollow(button.getAttribute("data-follow"));
    });
});

var editOverlay = document.querySelector(".edit-overlay");
var editForm = editOverlay ? editOverlay.querySelector(".edit-modal") : null;

function closeEdit() {
    if (!editOverlay) return;
    editOverlay.classList.remove("is-on");
    editOverlay.querySelectorAll(".edit-fold").forEach(function (fold) {
        fold.classList.remove("is-on");
        var body = fold.querySelector(".edit-fold-body");
        if (body) body.hidden = true;
    });
}

function openEdit() {
    if (!editOverlay || !editForm) return;
    editOverlay.classList.add("is-on");
}

if (editOverlay && editForm) {
    document.querySelectorAll(".details-edit").forEach(function (button) {
        button.addEventListener("click", openEdit);
    });
    editOverlay.addEventListener("click", function (event) {
        if (event.target === editOverlay) closeEdit();
    });
    editForm.querySelector(".edit-cancel").addEventListener("click", closeEdit);
    editForm.querySelector(".edit-phone-clear").addEventListener("click", function () {
        editForm.phone.value = "";
        editForm.phone.focus();
    });
    editForm.querySelector(".edit-pass-change").addEventListener("click", function () {
        var field = editForm.currentPassword;
        field.readOnly = false;
        field.required = true;
        editForm.newPassword.required = true;
        editForm.querySelector(".edit-pass-new").hidden = false;
        field.focus();
    });
    editForm.querySelectorAll(".edit-fold-toggle").forEach(function (button) {
        button.addEventListener("click", function () {
            var fold = button.parentElement;
            var body = fold.querySelector(".edit-fold-body");
            var open = fold.classList.toggle("is-on");
            body.hidden = !open;
        });
    });
}

var addressOverlay = document.querySelector(".address-overlay");
var addressForm = addressOverlay ? addressOverlay.querySelector(".edit-modal") : null;
var addressList = document.querySelector(".details-address-list");

function closeAddress() {
    if (!addressOverlay) return;
    addressOverlay.classList.remove("is-on");
    if (addressForm) addressForm.reset();
}

function openAddress() {
    if (!addressOverlay) return;
    if (addressForm) addressForm.reset();
    addressOverlay.classList.add("is-on");
    var first = addressForm && addressForm.title;
    if (first) first.focus();
}

if (addressOverlay && addressForm) {
    document.querySelectorAll(".details-add").forEach(function (button) {
        button.addEventListener("click", openAddress);
    });
    addressOverlay.addEventListener("click", function (event) {
        if (event.target === addressOverlay) closeAddress();
    });
    addressForm.querySelector(".edit-cancel").addEventListener("click", closeAddress);
    addressForm.addEventListener("submit", function (event) {
        if (addressForm.hasAttribute("action")) return;
        event.preventDefault();
        var title = addressForm.title.value.trim() || "Address";
        var city = addressForm.city.value.trim();
        var street = addressForm.street.value.trim();
        var apt = addressForm.apt.value.trim();
        var post = addressForm.postcode.value.trim();
        var line = [street, apt, city, post].filter(Boolean).join(", ");
        if (!line) return;
        if (addressForm.primary.checked && addressList) {
            addressList.querySelectorAll("em").forEach(function (mark) { mark.remove(); });
        }
        var card = document.createElement("div");
        card.className = "details-address-card";
        var head = document.createElement("strong");
        head.textContent = title;
        var text = document.createElement("p");
        text.textContent = line;
        card.appendChild(head);
        card.appendChild(text);
        if (addressForm.primary.checked) {
            var mark = document.createElement("em");
            mark.textContent = "Primary";
            card.appendChild(mark);
        }
        if (addressList) addressList.appendChild(card);
        closeAddress();
    });
}

var topupOverlay = document.querySelector(".topup-overlay");
var topupForm = topupOverlay ? topupOverlay.querySelector(".topup-modal") : null;
var topupKind = "money";

function syncTopupLabel() {
    if (!topupForm) return;
    var mark = topupForm.querySelector(".topup-submit-sum");
    if (!mark) return;
    var add = parseFloat(topupForm.amount.value);
    mark.textContent = topupKind === "money" && add > 0 ? add + "₼" : "";
}

function closeTopup() {
    if (!topupOverlay) return;
    topupOverlay.classList.remove("is-on");
}

function openTopup() {
    if (!topupOverlay || !topupForm) return;
    topupForm.reset();
    topupForm.agree.checked = true;
    topupKind = "money";
    topupForm.querySelectorAll(".topup-tabs button").forEach(function (button) {
        button.classList.toggle("is-on", button.getAttribute("data-topup") === "money");
    });
    topupForm.querySelector(".topup-money").hidden = false;
    topupForm.querySelector(".topup-cert").hidden = true;
    topupOverlay.classList.add("is-on");
    syncTopupLabel();
    if (topupForm.amount) topupForm.amount.focus();
}

if (topupOverlay && topupForm) {
    document.querySelectorAll(".cabinet-replenish").forEach(function (link) {
        link.addEventListener("click", function (event) {
            event.preventDefault();
            openTopup();
        });
    });
    topupOverlay.addEventListener("click", function (event) {
        if (event.target === topupOverlay) closeTopup();
    });
    topupForm.querySelectorAll(".topup-tabs button").forEach(function (button) {
        button.addEventListener("click", function () {
            topupKind = button.getAttribute("data-topup");
            topupForm.querySelectorAll(".topup-tabs button").forEach(function (tab) {
                tab.classList.toggle("is-on", tab === button);
            });
            topupForm.querySelector(".topup-money").hidden = topupKind !== "money";
            topupForm.querySelector(".topup-cert").hidden = topupKind !== "certificate";
            if (topupKind === "money") topupForm.amount.focus();
            else topupForm.code.focus();
            syncTopupLabel();
        });
    });
    topupForm.amount.addEventListener("input", function () {
        topupForm.amount.value = topupForm.amount.value.replace(/[^\d.,]/g, "").replace(",", ".");
        syncTopupLabel();
    });
    topupForm.addEventListener("submit", function (event) {
        topupForm.kind.value = topupKind;
        var ready = topupKind === "certificate"
            ? topupForm.code.value.trim() !== ""
            : parseFloat(topupForm.amount.value) >= 1;
        if (!ready || !topupForm.agree.checked) {
            event.preventDefault();
            showToast(topupKind === "certificate" ? "Enter the certificate code" : "The amount must be at least 1 ₼");
        }
    });
}

var cookOverlay = document.querySelector(".cook-overlay");
var cookForm = cookOverlay ? cookOverlay.querySelector(".cook-modal") : null;
var cookList = document.querySelector(".profile-cook-list");
var cookOther = cookForm ? cookForm.querySelector(".cook-other") : null;
var cookSaved = [];
var cookOtherText = "";

function saveCommunityProfile() {
    var body = new URLSearchParams();
    var bio = document.querySelector(".profile-bio");
    var text = bio ? bio.textContent.trim() : "";
    body.set("bio", text === "Tell about yourself" ? "" : text);
    cookSaved.forEach(function (name) {
        body.append("cookMethods", name === "Other" && cookOtherText ? cookOtherText : name);
    });
    body.set("__RequestVerificationToken", document.documentElement.getAttribute("data-csrf") || "");
    return fetch("/Community/UpdateProfile", {
        method: "POST",
        headers: { "Content-Type": "application/x-www-form-urlencoded", "X-Requested-With": "XMLHttpRequest" },
        body: body.toString()
    }).then(function (response) {
        if (!response.ok) {
            return response.json().then(function (result) {
                showToast(result && result.errors && result.errors.length ? result.errors[0] : "Something went wrong. Please try again.");
                return false;
            }).catch(function () {
                showToast("Something went wrong. Please try again.");
                return false;
            });
        }
        return true;
    }).catch(function () {
        showToast("The API is not running.");
        return false;
    });
}

function cookOptionOn(button) {
    return button.classList.contains("is-on");
}

function showCookOther() {
    if (!cookForm || !cookOther) return;
    var other = cookForm.querySelector('.cook-option[data-name="Other"]');
    var on = other && cookOptionOn(other);
    cookOther.hidden = !on;
    if (on) {
        cookForm.other.value = cookOtherText;
        cookForm.other.focus();
    }
}

function fillCookOptions() {
    if (!cookForm) return;
    cookForm.querySelectorAll(".cook-option").forEach(function (button) {
        var name = button.getAttribute("data-name");
        button.classList.toggle("is-on", cookSaved.indexOf(name) !== -1);
    });
    if (cookForm.other) cookForm.other.value = cookOtherText;
    showCookOther();
}

var cookGuides = {
    "Espresso machine": "/Journal/Article/inside-an-espresso-machine-pressure-heat-and-a-few-seconds",
    "Moka pot": "/Journal/Article/moka-pot-strong-stovetop-coffee-without-a-machine",
    "AeroPress": "/Journal/Article/aeropress-the-traveller-s-favourite-brewer",
    "Drip coffee maker": "/Journal/Article/drip-coffee-maker-how-to-get-a-better-pot-every-morning",
    "Pour over": "/Journal/Article/pour-over-at-home-a-simple-start",
    "Capsules": "/Journal/Article/coffee-capsules-fast-consistent-and-how-to-do-it-well",
    "Cezve": "/Journal/Article/turkish-coffee-how-to-make-it-in-a-cezve",
    "Immersion dripper": "/Journal/Article/immersion-dripper-easy-sweet-coffee-with-no-skills-needed",
    "Drip bags": "/Journal/Article/drip-bags-good-coffee-when-you-are-on-the-move",
    "French press": "/Journal/Article/french-press-a-simple-method-for-a-full-bodied-cup",
    "In a cup": "/Journal/Article/coffee-in-a-cup-brewing-without-any-equipment"
};

function renderCookList() {
    if (!cookList || !cookForm) return;
    cookList.innerHTML = "";
    cookSaved.forEach(function (name) {
        var option = cookForm.querySelector('.cook-option[data-name="' + name + '"]');
        var icon = option ? option.querySelector("svg") : null;
        var title = name === "Other" && cookOtherText ? cookOtherText : name;
        var card = document.createElement("div");
        card.className = "profile-cook-item";
        if (icon) {
            card.appendChild(icon.cloneNode(true));
        } else {
            var plus = document.createElement("span");
            plus.className = "profile-cook-plus";
            plus.textContent = "+";
            card.appendChild(plus);
        }
        var body = document.createElement("div");
        var strong = document.createElement("strong");
        strong.textContent = title;
        body.appendChild(strong);
        if (name !== "Other" && cookGuides[name]) {
            var link = document.createElement("a");
            link.href = cookGuides[name];
            link.textContent = "Open the guide";
            body.appendChild(link);
        }
        card.appendChild(body);
        cookList.appendChild(card);
    });
}

function closeCook() {
    if (!cookOverlay) return;
    cookOverlay.classList.remove("is-on");
}

function openCook() {
    if (!cookOverlay || !cookForm) return;
    fillCookOptions();
    cookOverlay.classList.add("is-on");
}

if (cookOverlay && cookForm && cookList) {
    var cookKnown = [];
    cookForm.querySelectorAll(".cook-option").forEach(function (button) {
        cookKnown.push(button.getAttribute("data-name"));
    });
    (cookList.getAttribute("data-cook") || "").split("|").forEach(function (name) {
        if (!name) return;
        if (cookKnown.indexOf(name) !== -1) {
            cookSaved.push(name);
        } else if (cookSaved.indexOf("Other") === -1) {
            cookSaved.push("Other");
            cookOtherText = name;
        }
    });
    renderCookList();
}

if (cookOverlay && cookForm) {
    document.querySelectorAll(".profile-cook-edit").forEach(function (button) {
        button.addEventListener("click", openCook);
    });
    cookOverlay.addEventListener("click", function (event) {
        if (event.target === cookOverlay) closeCook();
    });
    cookForm.querySelector(".edit-cancel").addEventListener("click", closeCook);
    cookForm.querySelectorAll(".cook-option").forEach(function (button) {
        button.addEventListener("click", function () {
            button.classList.toggle("is-on");
            if (button.getAttribute("data-name") === "Other") showCookOther();
        });
    });
    cookForm.addEventListener("submit", function (event) {
        event.preventDefault();
        cookSaved = [];
        cookForm.querySelectorAll(".cook-option.is-on").forEach(function (button) {
            cookSaved.push(button.getAttribute("data-name"));
        });
        cookOtherText = cookForm.other ? cookForm.other.value.trim() : "";
        renderCookList();
        closeCook();
        saveCommunityProfile();
    });
}

var aboutBox = document.querySelector(".profile-about");
var aboutBio = aboutBox ? aboutBox.querySelector(".profile-bio") : null;
var aboutEdit = aboutBox ? aboutBox.querySelector(".profile-edit") : null;
var aboutForm = aboutBox ? aboutBox.querySelector(".profile-bio-form") : null;
var aboutEmpty = "Tell about yourself";

function closeAbout() {
    if (!aboutBox || !aboutForm) return;
    aboutBox.classList.remove("is-on");
    aboutForm.hidden = true;
}

function openAbout() {
    if (!aboutBox || !aboutForm || !aboutBio) return;
    var now = aboutBio.textContent.trim();
    aboutForm.bio.value = now === aboutEmpty ? "" : now;
    aboutBox.classList.add("is-on");
    aboutForm.hidden = false;
    aboutForm.bio.focus();
}

if (aboutBox && aboutForm && aboutEdit) {
    aboutEdit.addEventListener("click", openAbout);
    aboutForm.addEventListener("submit", function (event) {
        event.preventDefault();
        var text = aboutForm.bio.value.trim();
        if (aboutBio) aboutBio.textContent = text || aboutEmpty;
        closeAbout();
        saveCommunityProfile();
    });
}


document.querySelectorAll(".cabinet-nav").forEach(function (nav) {
    var tabs = nav.querySelectorAll(".cabinet-tab");
    var panes = document.querySelectorAll(".cabinet-pane");
    function showPane(name) {
        tabs.forEach(function (tab) {
            tab.classList.toggle("is-on", tab.getAttribute("data-pane") === name);
        });
        panes.forEach(function (pane) {
            pane.hidden = pane.getAttribute("data-pane") !== name;
        });
    }
    tabs.forEach(function (tab) {
        tab.addEventListener("click", function (event) {
            event.preventDefault();
            var name = tab.getAttribute("data-pane");
            showPane(name);
            if (history.replaceState) {
                history.replaceState(null, "", "#" + name);
            }
        });
    });
    var hash = (location.hash || "").replace("#", "");
    if (hash && nav.querySelector('.cabinet-tab[data-pane="' + hash + '"]')) {
        showPane(hash);
    }
    window.addEventListener("hashchange", function () {
        var next = (location.hash || "").replace("#", "");
        if (next && nav.querySelector('.cabinet-tab[data-pane="' + next + '"]')) {
            showPane(next);
        }
    });
});

document.querySelectorAll(".notice-tabs").forEach(function (tabs) {
    var buttons = tabs.querySelectorAll("button");
    var items = document.querySelectorAll(".notice-list .notice");
    var empty = document.querySelector(".notice-empty");
    function showKind(kind) {
        var visible = 0;
        items.forEach(function (item) {
            var on = kind === "all" || item.getAttribute("data-kind") === kind;
            item.hidden = !on;
            if (on) visible += 1;
        });
        if (empty) empty.hidden = visible > 0;
    }
    buttons.forEach(function (button) {
        button.addEventListener("click", function () {
            var kind = button.getAttribute("data-kind");
            buttons.forEach(function (item) {
                var on = item === button;
                item.classList.toggle("is-on", on);
                item.setAttribute("aria-pressed", on ? "true" : "false");
            });
            showKind(kind);
        });
    });
});

document.querySelectorAll(".profile-composer").forEach(function (box) {
    var openBtn = box.querySelector(".profile-compose");
    var form = box.querySelector(".profile-post");
    var closeBtn = box.querySelector(".profile-post-close");
    var text = form.querySelector('input[name="text"]');
    function open() {
        box.classList.add("is-on");
        form.hidden = false;
        if (text) text.focus();
    }
    function close() {
        box.classList.remove("is-on");
        form.hidden = true;
        form.classList.remove("has-product");
        form.reset();
        var review = form.querySelector(".profile-review");
        if (review) {
            review.hidden = true;
            review.querySelectorAll(".profile-star").forEach(function (star) {
                star.classList.remove("is-on");
            });
            var brewToggle = review.querySelector(".profile-brew-toggle");
            var more = review.querySelector(".profile-recipe-more");
            var extra = review.querySelector(".profile-recipe-extra");
            if (brewToggle) {
                brewToggle.textContent = "Brewing method";
                brewToggle.style.color = "";
            }
            if (more) more.classList.remove("is-on");
            if (extra) extra.hidden = true;
        }
        box.querySelectorAll(".profile-pick").forEach(function (pick) {
            pick.classList.remove("is-on");
            var drop = pick.querySelector(".profile-pick-drop");
            if (drop) drop.hidden = true;
        });
        var chips = form.querySelector(".profile-tag-chips");
        var tagPick = form.querySelector(".profile-tag-pick");
        var tagDrop = form.querySelector(".profile-tag-drop");
        if (chips) chips.innerHTML = "";
        if (tagPick) tagPick.classList.remove("is-on");
        if (tagDrop) {
            tagDrop.hidden = true;
            tagDrop.querySelectorAll("button").forEach(function (item) { item.hidden = false; });
        }
    }
    openBtn.addEventListener("click", open);
    closeBtn.addEventListener("click", close);
    form.addEventListener("submit", function (event) {
        var value = text ? text.value.trim() : "";
        if (!value) {
            event.preventDefault();
            if (text) text.focus();
        }
    });
});

document.querySelectorAll(".profile-pick").forEach(function (pick) {
    var form = pick.closest(".profile-post");
    var search = pick.querySelector(".profile-pick-search");
    var drop = pick.querySelector(".profile-pick-drop");
    var hidden = pick.querySelector('input[name="product"]');
    var hiddenId = pick.querySelector('input[name="productId"]');
    var items = pick.querySelectorAll(".profile-pick-list button");
    var review = form ? form.querySelector(".profile-review") : null;
    var reviewImg = review ? review.querySelector(".profile-review-img") : null;
    var reviewName = review ? review.querySelector(".profile-review-name") : null;
    var reviewClear = review ? review.querySelector(".profile-review-clear") : null;
    function filter() {
        var q = search.value.trim().toLowerCase();
        items.forEach(function (item) {
            var name = (item.getAttribute("data-name") || "").toLowerCase();
            item.hidden = q !== "" && name.indexOf(q) === -1;
        });
    }
    function openPick() {
        pick.classList.add("is-on");
        drop.hidden = false;
    }
    function closePick() {
        pick.classList.remove("is-on");
        drop.hidden = true;
        if (hidden.value) {
            search.value = hidden.value;
            items.forEach(function (item) { item.hidden = false; });
            return;
        }
        search.value = "";
        filter();
    }
    function clearProduct() {
        hidden.value = "";
        if (hiddenId) hiddenId.value = "";
        search.value = "";
        items.forEach(function (item) { item.hidden = false; });
        if (form) form.classList.remove("has-product");
        if (review) review.hidden = true;
        if (reviewImg) reviewImg.removeAttribute("src");
        if (reviewName) reviewName.textContent = "";
        if (review) {
            var rating = review.querySelector('input[name="rating"]');
            var brew = review.querySelector('input[name="brew"]');
            var recipe = review.querySelector('input[name="recipe"]');
            var toggle = review.querySelector(".profile-brew-toggle");
            var extra = review.querySelector(".profile-recipe-extra");
            var more = review.querySelector(".profile-recipe-more");
            if (rating) rating.value = "";
            if (brew) brew.value = "";
            if (recipe) recipe.value = "";
            if (toggle) toggle.textContent = "Brewing method";
            review.querySelectorAll(".profile-star").forEach(function (star) {
                star.classList.remove("is-on");
            });
            review.querySelectorAll(".profile-recipe-extra input").forEach(function (field) {
                field.value = "";
            });
            if (extra) extra.hidden = true;
            if (more) more.classList.remove("is-on");
        }
        closePick();
    }
    search.addEventListener("focus", openPick);
    search.addEventListener("pointerdown", function (event) {
        event.stopPropagation();
        openPick();
    });
    search.addEventListener("input", function () {
        hidden.value = "";
        if (hiddenId) hiddenId.value = "";
        openPick();
        filter();
    });
    search.addEventListener("keydown", function (event) {
        if (event.key === "Enter") event.preventDefault();
    });
    items.forEach(function (item) {
        item.addEventListener("click", function () {
            var name = item.getAttribute("data-name") || "";
            var src = item.getAttribute("data-img") || "";
            hidden.value = name;
            if (hiddenId) hiddenId.value = item.getAttribute("data-id") || "";
            search.value = name;
            items.forEach(function (row) { row.hidden = false; });
            if (reviewImg) reviewImg.src = src;
            if (reviewName) reviewName.textContent = name;
            if (review) review.hidden = false;
            if (form) form.classList.add("has-product");
            closePick();
        });
    });
    if (reviewClear) reviewClear.addEventListener("click", clearProduct);
    document.addEventListener("pointerdown", function (event) {
        if (!pick.contains(event.target)) closePick();
    });
});

document.querySelectorAll(".profile-review").forEach(function (review) {
    var stars = review.querySelectorAll(".profile-star");
    var rating = review.querySelector('input[name="rating"]');
    var brewPick = review.querySelector(".profile-brew-pick");
    var brewToggle = review.querySelector(".profile-brew-toggle");
    var brewDrop = review.querySelector(".profile-brew-drop");
    var brewHidden = review.querySelector('input[name="brew"]');
    var more = review.querySelector(".profile-recipe-more");
    var extra = review.querySelector(".profile-recipe-extra");
    stars.forEach(function (star) {
        star.addEventListener("click", function () {
            var value = Number(star.getAttribute("data-star") || 0);
            if (rating) rating.value = String(value);
            stars.forEach(function (item) {
                item.classList.toggle("is-on", Number(item.getAttribute("data-star") || 0) <= value);
            });
        });
    });
    function closeBrew() {
        if (!brewPick) return;
        brewPick.classList.remove("is-on");
        if (brewDrop) brewDrop.hidden = true;
    }
    if (brewToggle && brewDrop) {
        brewToggle.addEventListener("click", function (event) {
            event.stopPropagation();
            var open = brewPick.classList.toggle("is-on");
            brewDrop.hidden = !open;
        });
        brewDrop.querySelectorAll("button").forEach(function (item) {
            item.addEventListener("click", function () {
                var name = item.getAttribute("data-name") || "";
                if (brewHidden) brewHidden.value = name;
                brewToggle.textContent = name;
                brewToggle.style.color = "inherit";
                closeBrew();
            });
        });
        document.addEventListener("pointerdown", function (event) {
            if (!brewPick.contains(event.target)) closeBrew();
        });
    }
    if (more && extra) {
        more.addEventListener("click", function () {
            var open = extra.hidden;
            extra.hidden = !open;
            more.classList.toggle("is-on", open);
        });
    }
});


document.querySelectorAll(".profile-tag-pick").forEach(function (pick) {
    var search = pick.querySelector(".profile-tag-search");
    var drop = pick.querySelector(".profile-tag-drop");
    var hidden = pick.querySelector('input[name="tags"]');
    var chips = pick.parentElement.querySelector(".profile-tag-chips");
    var items = pick.querySelectorAll(".profile-tag-drop button");
    function selected() {
        return hidden.value ? hidden.value.split("|") : [];
    }
    function writeTags(list) {
        hidden.value = list.join("|");
    }
    function filter() {
        var q = search.value.trim().toLowerCase();
        var taken = selected();
        var visible = 0;
        items.forEach(function (item) {
            var name = item.getAttribute("data-name") || "";
            var hide = taken.indexOf(name) !== -1 || (q !== "" && name.toLowerCase().indexOf(q) === -1);
            item.hidden = hide;
            if (!hide) visible += 1;
        });
        drop.hidden = visible === 0;
        pick.classList.toggle("is-on", visible > 0);
    }
    function openPick() {
        pick.classList.add("is-on");
        filter();
    }
    function closePick() {
        pick.classList.remove("is-on");
        drop.hidden = true;
        search.value = "";
    }
    function addTag(name) {
        var list = selected();
        if (!name || list.indexOf(name) !== -1) return;
        list.push(name);
        writeTags(list);
        var chip = document.createElement("span");
        chip.className = "profile-tag-chip";
        chip.setAttribute("data-name", name);
        chip.appendChild(document.createTextNode(name));
        var remove = document.createElement("button");
        remove.type = "button";
        remove.setAttribute("aria-label", "Remove " + name);
        remove.textContent = "×";
        remove.addEventListener("click", function () {
            chip.remove();
            writeTags(selected().filter(function (item) { return item !== name; }));
        });
        chip.appendChild(remove);
        chips.appendChild(chip);
        closePick();
    }
    search.addEventListener("focus", openPick);
    search.addEventListener("pointerdown", function (event) {
        event.stopPropagation();
        openPick();
    });
    search.addEventListener("input", openPick);
    search.addEventListener("keydown", function (event) {
        if (event.key !== "Enter") return;
        event.preventDefault();
        var match = Array.prototype.filter.call(items, function (item) { return !item.hidden; });
        if (match.length === 1) addTag(match[0].getAttribute("data-name") || "");
    });
    items.forEach(function (item) {
        item.addEventListener("click", function () {
            addTag(item.getAttribute("data-name") || "");
        });
    });
    document.addEventListener("pointerdown", function (event) {
        if (!pick.contains(event.target)) {
            pick.classList.remove("is-on");
            drop.hidden = true;
        }
    });
});

document.querySelectorAll("[data-autosubmit]").forEach(function (field) {
    field.addEventListener("change", function () {
        if (field.files && field.files.length) field.form.submit();
    });
});

var readingKey = "janz-reading";
var readingEmptyCopy = {
    progress: { title: "Nothing in progress", text: "Open an article and scroll about halfway" },
    read: { title: "Nothing read yet", text: "Finish an article to keep it here" },
    saved: { title: "No bookmarks", text: "Save an article from the journal" }
};

function loadReading() {
    try {
        return JSON.parse(localStorage.getItem(readingKey) || "{}");
    } catch (error) {
        return {};
    }
}

function saveReading(map) {
    localStorage.setItem(readingKey, JSON.stringify(map));
}

function articleMeta(root) {
    return {
        id: root.getAttribute("data-id"),
        title: root.getAttribute("data-title"),
        topic: root.getAttribute("data-topic"),
        date: root.getAttribute("data-date"),
        lead: root.getAttribute("data-lead"),
        image: root.getAttribute("data-image"),
        href: root.getAttribute("data-href") || "/Journal/Article",
        likes: root.getAttribute("data-likes") || "0",
        comments: root.getAttribute("data-comments") || "0",
        views: root.getAttribute("data-views") || "0",
        mins: root.getAttribute("data-mins") || ""
    };
}

function rememberArticle(root, patch) {
    var id = root.getAttribute("data-id");
    if (!id) return;
    var map = loadReading();
    var cur = map[id] || articleMeta(root);
    if (patch.status === "progress" && cur.status === "read") {
        delete patch.status;
    }
    Object.keys(patch).forEach(function (key) {
        cur[key] = patch[key];
    });
    map[id] = cur;
    saveReading(map);
}

var articleRoot = document.getElementById("article");
if (articleRoot && articleRoot.getAttribute("data-id")) {

    function pageProgress() {
        var max = document.documentElement.scrollHeight - window.innerHeight;
        if (max <= 80) return 0;
        return Math.min(1, window.scrollY / max);
    }

    function trackArticle() {
        var progress = pageProgress();
        if (progress >= 0.9) {
            rememberArticle(articleRoot, { status: "read", progress: progress });
        } else if (progress >= 0.4) {
            rememberArticle(articleRoot, { status: "progress", progress: progress });
        }
    }

    window.addEventListener("scroll", trackArticle, { passive: true });

    rememberArticle(articleRoot, { seenAt: Date.now() });
}

function postArticle(url, data) {
    var body = new URLSearchParams(data);
    body.set("__RequestVerificationToken", document.documentElement.getAttribute("data-csrf") || "");
    return fetch(url, {
        method: "POST",
        headers: { "Content-Type": "application/x-www-form-urlencoded", "X-Requested-With": "XMLHttpRequest" },
        body: body.toString()
    }).then(function (response) {
        if (response.status === 401) {
            document.documentElement.setAttribute("data-auth", "0");
            openAuth();
            return null;
        }
        if (!response.ok) {
            showToast("Something went wrong. Please try again.");
            return null;
        }
        return response.json();
    }).catch(function () {
        showToast("The API is not running.");
        return null;
    });
}

document.addEventListener("click", function (event) {
    var like = event.target.closest("[data-article-like]");
    if (like && isSignedIn() && !like.disabled) {
        var id = like.getAttribute("data-article-like");
        var on = !like.classList.contains("is-on");
        like.disabled = true;
        postArticle("/Journal/Like", { id: id, on: on }).then(function (result) {
            like.disabled = false;
            if (!result) return;
            document.querySelectorAll('[data-article-like="' + id + '"]').forEach(function (button) {
                var count = button.querySelector("span");
                var was = button.classList.contains("is-on");
                button.classList.toggle("is-on", on);
                button.setAttribute("aria-pressed", on ? "true" : "false");
                if (count && was !== on) count.textContent = Math.max(0, (parseInt(count.textContent, 10) || 0) + (on ? 1 : -1));
            });
        });
        return;
    }
    var save = event.target.closest("[data-article-save]");
    if (save && isSignedIn() && !save.disabled) {
        var saveId = save.getAttribute("data-article-save");
        var saveOn = !save.classList.contains("is-on");
        save.disabled = true;
        postArticle("/Journal/Save", { id: saveId, on: saveOn }).then(function (result) {
            save.disabled = false;
            if (!result) return;
            document.querySelectorAll('[data-article-save="' + saveId + '"]').forEach(function (button) {
                button.classList.toggle("is-on", saveOn);
                button.setAttribute("aria-pressed", saveOn ? "true" : "false");
            });
            showToast(saveOn ? "Saved to notes" : "Removed from notes");
        });
        return;
    }
    var vote = event.target.closest("[data-comment-vote]");
    if (vote && isSignedIn() && !vote.disabled) {
        var box = vote.closest(".article-comment-votes");
        var value = parseInt(vote.getAttribute("data-comment-vote"), 10);
        if (vote.classList.contains("is-on")) value = 0;
        vote.disabled = true;
        postArticle("/Journal/Vote", { commentId: box.getAttribute("data-comment"), value: value }).then(function (result) {
            vote.disabled = false;
            if (!result) return;
            box.querySelectorAll("[data-comment-vote]").forEach(function (button) {
                var mine = parseInt(button.getAttribute("data-comment-vote"), 10) === result.myVote;
                button.classList.toggle("is-on", mine);
                button.setAttribute("aria-pressed", mine ? "true" : "false");
                var count = button.querySelector("span");
                if (count) count.textContent = button.getAttribute("data-comment-vote") === "1" ? result.likes : result.dislikes;
            });
        });
    }
});

var readingBoard = document.querySelector(".reading-board");
if (readingBoard) {
    var readingTabs = readingBoard.querySelectorAll("[data-read]");
    var readingList = readingBoard.querySelector(".reading-list");
    var readingEmpty = readingBoard.querySelector(".reading-empty");
    var readingFilter = "progress";
    var savedSource = document.getElementById("saved-articles");
    var savedArticles = [];
    try { savedArticles = JSON.parse(savedSource ? savedSource.textContent : "[]"); } catch (error) { savedArticles = []; }

    function readingItems() {
        var map = loadReading();
        return Object.keys(map).map(function (id) {
            return map[id];
        });
    }

    function renderReading() {
        var items = readingItems();
        var counts = { progress: 0, read: 0, saved: savedArticles.length };
        items.forEach(function (item) {
            if (item.status === "read") counts.read += 1;
            else if (item.status === "progress") counts.progress += 1;
        });
        readingTabs.forEach(function (tab) {
            var key = tab.getAttribute("data-read");
            tab.classList.toggle("is-on", key === readingFilter);
            var count = tab.querySelector("span");
            if (count) count.textContent = counts[key] || 0;
        });
        var shown = readingFilter === "saved" ? savedArticles : items.filter(function (item) {
            return item.status === readingFilter;
        });
        readingList.innerHTML = "";
        shown.forEach(function (item) {
            var card = document.createElement("a");
            card.className = "journal-card";
            card.href = item.href || "/Journal/Article";
            card.innerHTML =
                '<img src="' + escapeHtml(item.image || "/images/aroma-kit.png") + '" alt="" />' +
                '<div class="body">' +
                    '<div class="meta">' + escapeHtml(item.topic || "") + "<br>" + escapeHtml(item.date || "") + "</div>" +
                    "<h3>" + escapeHtml(item.title || "Article") + "</h3>" +
                    "<p>" + escapeHtml(item.lead || "") + "</p>" +
                    '<div class="journal-stats">' +
                        '<span><svg viewBox="0 0 24 24"><path d="M8 11v9H5.2A1.2 1.2 0 0 1 4 18.8v-6.6A1.2 1.2 0 0 1 5.2 11H8zm0 0l3.2-6.1A2.2 2.2 0 0 1 13.2 4h.3A2.5 2.5 0 0 1 16 6.5V11h3.7a1.8 1.8 0 0 1 1.8 2.1l-1.2 6.2A1.8 1.8 0 0 1 18.5 21H8" fill="none" stroke="currentColor" stroke-width="1.6" stroke-linejoin="round"/></svg>' + (item.likes || "0") + "</span>" +
                        '<span><svg viewBox="0 0 24 24"><path d="M5 6.5h14v9.2H9.2L5 19.2V6.5z" fill="none" stroke="currentColor" stroke-width="1.6" stroke-linejoin="round"/></svg>' + (item.comments || "0") + "</span>" +
                        '<span><svg viewBox="0 0 24 24"><circle cx="12" cy="12" r="7.2" fill="none" stroke="currentColor" stroke-width="1.6"/><circle cx="12" cy="12" r="2.2" fill="none" stroke="currentColor" stroke-width="1.6"/></svg>' + (item.views || "0") + "</span>" +
                        '<span><svg viewBox="0 0 24 24"><circle cx="12" cy="12" r="7.2" fill="none" stroke="currentColor" stroke-width="1.6"/><path d="M12 8v4.2l2.6 1.6" fill="none" stroke="currentColor" stroke-width="1.6" stroke-linecap="round"/></svg>' + (item.mins || "") + "</span>" +
                    "</div>" +
                "</div>";
            readingList.appendChild(card);
        });
        readingBoard.classList.toggle("is-empty", shown.length === 0);
        if (readingEmpty) {
            var copy = readingEmptyCopy[readingFilter] || readingEmptyCopy.progress;
            var title = readingEmpty.querySelector("h2");
            var text = readingEmpty.querySelector("p");
            if (title) title.textContent = copy.title;
            if (text) text.textContent = copy.text;
        }
    }

    readingTabs.forEach(function (tab) {
        tab.addEventListener("click", function () {
            readingFilter = tab.getAttribute("data-read");
            renderReading();
        });
    });
    renderReading();
}


function bindVotes(box) {
    var like = box.querySelector(":scope > .feed-like");
    var dislike = box.querySelector(":scope > .feed-dislike");
    var score = box.querySelector(":scope > .feed-score");
    if (!like || !dislike || !score) return;
    var note = box.closest(".feed-note");
    var card = box.closest(".feed-card");
    var kind = note ? "comment" : "post";
    var id = note ? note.getAttribute("data-comment-id") : card.getAttribute("data-post-id");

    function send(value) {
        if (!isSignedIn()) {
            openAuth();
            return;
        }
        postArticle("/Community/Vote", { kind: kind, id: id, value: value }).then(function (result) {
            if (!result) return;
            score.textContent = result.score;
            like.classList.toggle("is-on", result.myVote === 1);
            like.setAttribute("aria-pressed", result.myVote === 1 ? "true" : "false");
            dislike.classList.toggle("is-on", result.myVote === -1);
            dislike.setAttribute("aria-pressed", result.myVote === -1 ? "true" : "false");
        });
    }

    like.addEventListener("click", function () {
        send(like.classList.contains("is-on") ? 0 : 1);
    });
    dislike.addEventListener("click", function () {
        send(dislike.classList.contains("is-on") ? 0 : -1);
    });
}

function bindNote(note, card) {
    var actions = note.querySelector(":scope > .feed-note-actions");
    if (actions) bindVotes(actions);

    var reply = actions ? actions.querySelector(".feed-reply") : null;
    var input = card.querySelector(".feed-comment input[type='text']");
    if (reply && input) {
        reply.addEventListener("click", function () {
            var name = note.querySelector(".feed-name a").textContent.trim();
            input.value = "@" + name + ", ";
            input.setAttribute("data-parent", note.getAttribute("data-comment-id"));
            input.focus();
            input.setSelectionRange(input.value.length, input.value.length);
        });
    }

    var toggle = note.querySelector(":scope > .feed-replies-toggle");
    var replies = note.querySelector(":scope > .feed-replies");
    if (toggle && replies) {
        toggle.addEventListener("click", function () {
            var open = replies.hidden;
            replies.hidden = !open;
            toggle.setAttribute("aria-expanded", open ? "true" : "false");
        });
    }
}

function escapeHtml(text) {
    var box = document.createElement("div");
    box.textContent = text;
    return box.innerHTML;
}

var shareOverlay = null;
var toastBox = null;
var toastTimer = null;

function postLink(card, note) {
    return location.origin + "/Community#post-" + card.getAttribute("data-post-id");
}

function openShare(link) {
    if (!shareOverlay) return;
    var field = shareOverlay.querySelector(".share-link");
    field.value = link;
    var text = encodeURIComponent(link);
    shareOverlay.querySelector("[data-share='telegram']").href = "https://t.me/share/url?url=" + text;
    shareOverlay.querySelector("[data-share='whatsapp']").href = "https://wa.me/?text=" + text;
    shareOverlay.querySelector("[data-share='facebook']").href = "https://www.facebook.com/sharer/sharer.php?u=" + text;
    if (navigator.clipboard) {
        navigator.clipboard.writeText(link).catch(function () {});
    }
    shareOverlay.classList.add("is-on");
}

function closeShare() {
    if (shareOverlay) shareOverlay.classList.remove("is-on");
}

function showToast(text) {
    if (!toastBox) return;
    toastBox.querySelector(".toast-text").textContent = text;
    toastBox.classList.remove("is-on");
    void toastBox.offsetWidth;
    toastBox.classList.add("is-on");
    clearTimeout(toastTimer);
    toastTimer = setTimeout(hideToast, 4000);
}

if (document.documentElement.getAttribute("data-toast")) {
    window.addEventListener("load", function () {
        showToast(document.documentElement.getAttribute("data-toast"));
    });
}

function hideToast() {
    if (toastBox) toastBox.classList.remove("is-on");
}

function closeMenus(except) {
    document.querySelectorAll(".feed-menu").forEach(function (menu) {
        if (menu !== except) menu.hidden = true;
    });
}

toastBox = document.createElement("div");
toastBox.className = "toast";
toastBox.setAttribute("role", "status");
toastBox.innerHTML =
    '<div class="toast-check" aria-hidden="true"><svg viewBox="0 0 24 24"><path d="M6 12.4l4 4L18 8" fill="none" stroke="currentColor" stroke-width="2.6" stroke-linecap="round" stroke-linejoin="round"/></svg></div>' +
    '<p class="toast-text"></p>' +
    '<div class="toast-bar"></div>' +
    '<button type="button" class="toast-close" aria-label="Close"><svg viewBox="0 0 24 24"><path d="M6 6l12 12M18 6L6 18" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round"/></svg></button>';
document.body.appendChild(toastBox);
toastBox.querySelector(".toast-close").addEventListener("click", function () {
    clearTimeout(toastTimer);
    hideToast();
});

if (document.querySelector(".feed-card")) {
    shareOverlay = document.createElement("div");
    shareOverlay.className = "share-overlay";
    shareOverlay.innerHTML =
        '<div class="share-modal" role="dialog" aria-modal="true" aria-labelledby="share-title">' +
            '<svg class="share-icon" viewBox="0 0 48 48" aria-hidden="true"><path d="M17 6h25v25H17z" fill="none" stroke="currentColor" stroke-width="3"/><path d="M11 13H6v29h29v-5" fill="none" stroke="currentColor" stroke-width="3"/></svg>' +
            '<h2 id="share-title">Link copied<br />to clipboard</h2>' +
            '<input class="share-link" type="text" readonly aria-label="Post link" />' +
            "<p>Share on social networks</p>" +
            '<div class="share-socials">' +
                '<a data-share="telegram" href="#" target="_blank" rel="noopener" aria-label="Telegram"><svg viewBox="0 0 24 24"><path d="M20.6 4.2L3.4 10.9c-.9.3-.9 1.6 0 1.9l4.2 1.4 1.6 5c.2.7 1.1.9 1.6.4l2.4-2.3 4.4 3.2c.6.4 1.4.1 1.6-.6l2.9-14.2c.2-.9-.7-1.7-1.5-1.5zM9.2 13.9l8.1-5.2-6.5 6.4-.3 3-1.3-4.2z" fill="currentColor"/></svg></a>' +
                '<a data-share="whatsapp" href="#" target="_blank" rel="noopener" aria-label="WhatsApp"><svg viewBox="0 0 24 24"><path d="M12 3.5a8.5 8.5 0 0 0-7.3 12.9L3.5 20.5l4.2-1.1A8.5 8.5 0 1 0 12 3.5z" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linejoin="round"/><path d="M9.3 8.3c.2-.4.4-.4.7-.4h.5c.2 0 .4.1.5.4l.7 1.6c.1.2 0 .5-.1.6l-.5.6c.6 1.1 1.5 2 2.6 2.6l.6-.5c.2-.2.4-.2.6-.1l1.6.7c.3.1.4.3.4.5v.5c0 .3-.1.5-.4.7-.5.4-1.2.6-1.8.5-2.9-.6-5.2-2.9-5.8-5.8-.1-.6.1-1.3.4-1.9z" fill="currentColor"/></svg></a>' +
                '<a data-share="facebook" href="#" target="_blank" rel="noopener" aria-label="Facebook"><svg viewBox="0 0 24 24"><path d="M13.4 20.5v-7h2.4l.4-2.8h-2.8V8.9c0-.8.2-1.4 1.4-1.4h1.5V5c-.3 0-1.1-.1-2.2-.1-2.2 0-3.6 1.3-3.6 3.7v2.1H8.1v2.8h2.4v7z" fill="currentColor"/></svg></a>' +
            "</div>" +
        "</div>";
    document.body.appendChild(shareOverlay);

    shareOverlay.addEventListener("click", function (event) {
        if (event.target === shareOverlay) closeShare();
    });
    shareOverlay.querySelector(".share-link").addEventListener("focus", function (event) {
        event.target.select();
    });

    document.addEventListener("keydown", function (event) {
        if (event.key === "Escape") {
            closeShare();
            closeMenus(null);
        }
    });

    document.addEventListener("click", function (event) {
        var more = event.target.closest(".feed-more");
        if (!more) {
            if (!event.target.closest(".feed-menu")) closeMenus(null);
            return;
        }

        var head = more.parentElement;
        var menu = head.querySelector(".feed-menu");
        if (!menu) {
            var card = more.closest(".feed-card");
            var note = more.closest(".feed-note");
            var mine = (note || card).getAttribute("data-mine") === "1";
            var userId = (note || card).getAttribute("data-user-id");
            menu = document.createElement("div");
            menu.className = "feed-menu";
            menu.hidden = true;
            menu.innerHTML = mine
                ? '<button type="button" data-menu="delete">Delete</button><button type="button" data-menu="share">Share</button>'
                : '<button type="button" data-menu="report">Report</button><button type="button" data-menu="block">Block</button><button type="button" data-menu="share">Share</button>';
            head.appendChild(menu);

            menu.addEventListener("click", function (menuEvent) {
                var item = menuEvent.target.closest("[data-menu]");
                if (!item) return;
                menu.hidden = true;
                var action = item.getAttribute("data-menu");
                var kind = note ? "comment" : "post";
                var id = note ? note.getAttribute("data-comment-id") : card.getAttribute("data-post-id");

                if (action === "share") {
                    openShare(postLink(card, note));
                    return;
                }

                if (!isSignedIn()) {
                    openAuth();
                    return;
                }

                if (action === "report") {
                    var reason = window.prompt("Why are you reporting this?");
                    if (!reason || !reason.trim()) return;
                    postArticle("/Community/Report", { kind: kind, id: id, reason: reason.trim() }).then(function (result) {
                        if (result) showToast("Thanks, we will look into it");
                    });
                } else if (action === "block") {
                    if (!window.confirm("Block this user? You will no longer see their posts and comments.")) return;
                    postArticle("/Community/Block", { userId: userId, on: true }).then(function (result) {
                        if (!result) return;
                        document.querySelectorAll('.feed-card[data-user-id="' + userId + '"], .feed-note[data-user-id="' + userId + '"]').forEach(function (item) {
                            item.remove();
                        });
                        showToast("You blocked the user");
                    });
                } else if (action === "delete") {
                    if (!window.confirm("Delete this " + kind + "?")) return;
                    postArticle(kind === "comment" ? "/Community/DeleteComment" : "/Community/DeletePost", { id: id }).then(function (result) {
                        if (!result) return;
                        var target = note || card;
                        var count = card.querySelector(".feed-comments span");
                        if (note && count) count.textContent = Math.max(0, (parseInt(count.textContent, 10) || 0) - 1 - note.querySelectorAll(".feed-replies .feed-note").length);
                        target.remove();
                    });
                }
            });
        }

        closeMenus(menu);
        menu.hidden = !menu.hidden;
    });
}

document.querySelectorAll(".feed-card").forEach(function (card) {
    var photos = card.querySelector("[data-feed-photos]");
    if (photos) {
        var frame = photos.querySelector(".feed-frame");
        var track = photos.querySelector(".feed-track");
        var dots = photos.querySelectorAll(".feed-dots button");
        var count = track.children.length;
        var index = 0;
        var startX = null;
        var deltaX = 0;

        function showPhoto(next) {
            index = Math.max(0, Math.min(count - 1, next));
            track.style.transform = "translateX(" + (-index * 100) + "%)";
            dots.forEach(function (dot, n) {
                dot.classList.toggle("is-on", n === index);
            });
        }

        dots.forEach(function (dot, n) {
            dot.addEventListener("click", function () {
                showPhoto(n);
            });
        });

        if (count > 1) {
            frame.addEventListener("pointerdown", function (event) {
                startX = event.clientX;
                deltaX = 0;
                frame.classList.add("is-dragging");
                frame.setPointerCapture(event.pointerId);
            });

            frame.addEventListener("pointermove", function (event) {
                if (startX === null) return;
                deltaX = event.clientX - startX;
                var shift = -index * frame.clientWidth + deltaX;
                track.style.transform = "translateX(" + shift + "px)";
            });

            function endDrag() {
                if (startX === null) return;
                frame.classList.remove("is-dragging");
                if (Math.abs(deltaX) > frame.clientWidth / 5) {
                    showPhoto(index + (deltaX < 0 ? 1 : -1));
                } else {
                    showPhoto(index);
                }
                startX = null;
            }

            frame.addEventListener("pointerup", endDrag);
            frame.addEventListener("pointercancel", endDrag);
        } else {
            frame.style.cursor = "default";
        }
    }

    var postActions = card.querySelector(".feed-actions");
    if (postActions) bindVotes(postActions);

    var save = card.querySelector(".feed-save");
    if (save) {
        save.addEventListener("click", function () {
            if (!isSignedIn()) return;
            var on = !save.classList.contains("is-on");
            postArticle("/Community/Save", { id: card.getAttribute("data-post-id"), on: on }).then(function (result) {
                if (!result) return;
                save.classList.toggle("is-on", on);
                save.setAttribute("aria-pressed", on ? "true" : "false");
            });
        });
    }

    var follow = card.querySelector(".feed-follow");
    if (follow) {
        follow.addEventListener("click", function () {
            if (!isSignedIn()) {
                openAuth();
                return;
            }
            var on = !follow.classList.contains("is-on");
            var userId = card.getAttribute("data-user-id");
            postArticle("/Community/Follow", { userId: userId, on: on }).then(function (result) {
                if (!result) return;
                document.querySelectorAll('.feed-card[data-user-id="' + userId + '"] .feed-follow').forEach(function (button) {
                    button.classList.toggle("is-on", on);
                    button.setAttribute("aria-pressed", on ? "true" : "false");
                    button.textContent = on ? "Following" : "Follow";
                });
            });
        });
    }

    var share = card.querySelector(".feed-share");
    if (share) {
        share.addEventListener("click", function () {
            openShare(postLink(card, null));
        });
    }

    var thread = card.querySelector(".feed-thread");
    var commentsBtn = card.querySelector(".feed-comments");
    var commentsCount = commentsBtn ? commentsBtn.querySelector("span") : null;

    function setThread(open) {
        if (!thread || !commentsBtn) return;
        thread.hidden = !open;
        commentsBtn.classList.toggle("is-on", open);
        commentsBtn.setAttribute("aria-expanded", open ? "true" : "false");
    }

    if (thread && commentsBtn) {
        commentsBtn.addEventListener("click", function () {
            setThread(thread.hidden);
        });
        thread.querySelectorAll(".feed-note").forEach(function (note) {
            bindNote(note, card);
        });
    }

    var comment = card.querySelector(".feed-comment");
    if (comment) {
        var text = comment.querySelector("input[type='text']");
        var file = comment.querySelector("input[type='file']");
        var photoBtn = comment.querySelector(".feed-comment-photo");

        if (file && photoBtn) {
            file.addEventListener("change", function () {
                photoBtn.classList.toggle("is-on", file.files.length > 0);
            });
        }

        comment.addEventListener("submit", function (event) {
            event.preventDefault();
            var value = text.value.trim();
            var hasPhoto = file && file.files.length > 0;
            if (!value && !hasPhoto) {
                text.focus();
                return;
            }
            if (!isSignedIn()) {
                openAuth();
                return;
            }

            var parent = text.getAttribute("data-parent");
            var form = new FormData();
            form.append("postId", card.getAttribute("data-post-id"));
            form.append("text", value);
            if (parent) form.append("parentId", parent);
            if (hasPhoto) form.append("image", file.files[0]);
            form.append("__RequestVerificationToken", document.documentElement.getAttribute("data-csrf") || "");

            fetch("/Community/Comment", { method: "POST", headers: { "X-Requested-With": "XMLHttpRequest" }, body: form }).then(function (response) {
                if (response.status === 401) {
                    document.documentElement.setAttribute("data-auth", "0");
                    openAuth();
                    return null;
                }
                if (!response.ok) {
                    return response.json().then(function (body) {
                        showToast(body && body.errors && body.errors.length ? body.errors[0] : "Something went wrong. Please try again.");
                        return null;
                    }).catch(function () {
                        showToast("Something went wrong. Please try again.");
                        return null;
                    });
                }
                return response.text();
            }).then(function (html) {
                if (!html) return;
                if (parent) {
                    sessionStorage.setItem("community-open", card.getAttribute("data-post-id"));
                    location.reload();
                    return;
                }
                var holder = document.createElement("div");
                holder.innerHTML = html.trim();
                var note = holder.firstElementChild;
                var empty = thread.querySelector(".feed-thread-empty");
                thread.insertBefore(note, empty);
                if (empty) empty.hidden = true;
                bindNote(note, card);
                setThread(true);
                if (commentsCount) commentsCount.textContent = (parseInt(commentsCount.textContent, 10) || 0) + 1;
                comment.reset();
                text.removeAttribute("data-parent");
                if (photoBtn) photoBtn.classList.remove("is-on");
            }).catch(function () {
                showToast("The API is not running.");
            });
        });
    }

    var openId = sessionStorage.getItem("community-open");
    if (openId && openId === card.getAttribute("data-post-id")) {
        sessionStorage.removeItem("community-open");
        setThread(true);
        card.scrollIntoView({ block: "center" });
    } else if (location.hash === "#post-" + card.getAttribute("data-post-id")) {
        setThread(true);
    }
});

var blacklistPane = document.querySelector(".cabinet-pane[data-pane='blacklist']");
var unblockOverlay = document.querySelector(".unblock-overlay");

if (blacklistPane && unblockOverlay) {
    var blacklistBoard = blacklistPane.querySelector(".blacklist-board");
    var blacklistSearch = blacklistPane.querySelector(".blacklist-search input");
    var blacklistNone = blacklistPane.querySelector(".blacklist-none");
    var blacklistEmpty = blacklistPane.querySelector(".blacklist-empty");
    var unblockTarget = null;

    function syncBlacklist() {
        var items = blacklistPane.querySelectorAll(".blacklist-item");
        var query = blacklistSearch.value.trim().toLowerCase();
        var shown = 0;
        items.forEach(function (item) {
            var match = item.textContent.toLowerCase().indexOf(query) !== -1;
            item.hidden = !match;
            if (match) shown++;
        });
        var hasUsers = items.length > 0;
        blacklistBoard.hidden = !hasUsers;
        blacklistEmpty.hidden = hasUsers;
        blacklistPane.classList.toggle("is-filled", hasUsers);
        blacklistNone.hidden = !hasUsers || shown > 0;
    }

    function closeUnblock() {
        unblockOverlay.classList.remove("is-on");
        unblockTarget = null;
    }

    blacklistSearch.addEventListener("input", syncBlacklist);

    blacklistPane.addEventListener("click", function (event) {
        var remove = event.target.closest(".blacklist-remove");
        if (!remove) return;
        unblockTarget = remove.closest(".blacklist-item");
        var name = unblockTarget.querySelector(".blacklist-name strong").textContent;
        unblockOverlay.querySelector(".unblock-avatar").textContent = unblockTarget.querySelector(".blacklist-avatar").textContent;
        unblockOverlay.querySelector(".unblock-name").textContent = name;
        unblockOverlay.querySelector(".unblock-role").textContent = unblockTarget.querySelector(".blacklist-who p").textContent;
        unblockOverlay.classList.add("is-on");
    });

    unblockOverlay.querySelector(".unblock-yes").addEventListener("click", function () {
        var target = unblockTarget;
        closeUnblock();
        if (!target) return;
        postArticle("/Community/Block", { userId: target.getAttribute("data-user-id"), on: false }).then(function (result) {
            if (!result) return;
            target.remove();
            syncBlacklist();
            showToast("The user was removed from the blacklist");
        });
    });

    unblockOverlay.querySelector(".unblock-no").addEventListener("click", closeUnblock);

    unblockOverlay.addEventListener("click", function (event) {
        if (event.target === unblockOverlay) closeUnblock();
    });

    document.addEventListener("keydown", function (event) {
        if (event.key === "Escape") closeUnblock();
    });

    syncBlacklist();
}


var journalGrid = document.querySelector(".journal-grid");

if (journalGrid && journalGrid.querySelector(".article-card")) {
    var journalCards = Array.prototype.slice.call(journalGrid.querySelectorAll(".article-card"));
    var journalNone = journalGrid.querySelector(".journal-none");
    var journalCount = document.querySelector(".journal-count");
    var journalSort = document.querySelector(".journal-toolbar select");
    var journalRecent = document.querySelector(".journal-recent");
    var journalRecentRow = journalRecent ? journalRecent.querySelector(".journal-recent-row") : null;
    var journalSection = document.getElementById("journal");
    var journalTopic = journalSection ? journalSection.getAttribute("data-topic") || "" : "";
    var journalTag = journalSection ? journalSection.getAttribute("data-tag") || "" : "";

    function cardTopics(card) {
        return (card.getAttribute("data-topic") || "").split(",").map(function (item) {
            return item.trim();
        });
    }

    function cardTags(card) {
        return (card.getAttribute("data-tags") || "").split(",").map(function (item) {
            return item.trim();
        });
    }

    document.querySelectorAll(".journal-topics a[data-topic]").forEach(function (link) {
        var topic = link.getAttribute("data-topic");
        var count = link.querySelector("span");
        if (!topic || !count) return;
        count.textContent = journalCards.filter(function (card) {
            return cardTopics(card).indexOf(topic) !== -1;
        }).length;
    });

    function sortValue(card, mode) {
        if (mode === "Most read") return parseInt(card.getAttribute("data-views"), 10) || 0;
        if (mode === "Most popular") return parseInt(card.getAttribute("data-likes"), 10) || 0;
        return card.getAttribute("data-sort") || "";
    }

    function renderJournal() {
        var mode = journalSort ? journalSort.value : "Newest first";
        var sorted = journalCards.slice().sort(function (a, b) {
            var x = sortValue(a, mode);
            var y = sortValue(b, mode);
            if (x === y) return 0;
            var asc = mode === "Oldest first";
            return (x > y ? 1 : -1) * (asc ? 1 : -1);
        });

        var shown = 0;
        sorted.forEach(function (card) {
            var match = (!journalTopic || cardTopics(card).indexOf(journalTopic) !== -1) &&
                        (!journalTag || cardTags(card).indexOf(journalTag) !== -1);
            card.hidden = !match;
            if (match) shown++;
            journalGrid.insertBefore(card, journalNone);
        });

        if (journalNone) journalNone.hidden = shown > 0;
        if (journalCount) journalCount.textContent = shown + (shown === 1 ? " article" : " articles");
    }

    document.querySelectorAll(".journal-topics a[data-topic]").forEach(function (link) {
        link.addEventListener("click", function (event) {
            event.preventDefault();
            journalTopic = link.getAttribute("data-topic");
            document.querySelectorAll(".journal-topics a[data-topic]").forEach(function (item) {
                item.classList.toggle("is-on", item === link);
            });
            renderJournal();
        });
    });

    document.querySelectorAll(".journal-tags a[data-tag]").forEach(function (link) {
        link.addEventListener("click", function (event) {
            event.preventDefault();
            var tag = link.getAttribute("data-tag");
            journalTag = journalTag === tag ? "" : tag;
            document.querySelectorAll(".journal-tags a[data-tag]").forEach(function (item) {
                item.classList.toggle("is-on", !!journalTag && item.getAttribute("data-tag") === journalTag);
            });
            renderJournal();
        });
    });

    if (journalSort) journalSort.addEventListener("change", renderJournal);

    function syncArticle(card) {
        var item = loadReading()[card.getAttribute("data-id")] || {};
        var bar = card.querySelector(".article-progress i");
        if (bar) {
            var progress = item.status === "read" ? 1 : (item.progress || 0);
            bar.style.width = Math.round(progress * 100) + "%";
            card.classList.toggle("is-started", progress > 0);
        }
    }

    function syncAll() {
        document.querySelectorAll(".article-card, .journal-hero[data-id]").forEach(syncArticle);
    }

    function bindArticle(card) {
        card.querySelectorAll("a[href^='/Journal/Article']").forEach(function (link) {
            link.addEventListener("click", function () {
                rememberArticle(card, { seenAt: Date.now() });
            });
        });
    }

    function renderRecent() {
        if (!journalRecent || !journalRecentRow) return;
        var map = loadReading();
        var seen = Object.keys(map).map(function (id) {
            return map[id];
        }).filter(function (item) {
            return item.seenAt;
        }).sort(function (a, b) {
            return b.seenAt - a.seenAt;
        }).slice(0, 6);

        journalRecentRow.innerHTML = "";
        seen.forEach(function (item) {
            var source = document.querySelector(".article-card[data-id='" + item.id + "'], .journal-hero[data-id='" + item.id + "']");
            var card;
            if (source && source.classList.contains("article-card")) {
                card = source.cloneNode(true);
                card.hidden = false;
            } else {
                card = document.createElement("div");
                card.className = "article-card";
                ["id", "title", "topic", "date", "lead", "image", "href", "likes", "comments", "views", "mins"].forEach(function (key) {
                    if (item[key]) card.setAttribute("data-" + key, item[key]);
                });
                var stats = source ? source.querySelector(".article-stats").outerHTML : "";
                card.innerHTML =
                    '<a class="article-card-photo" href="' + (item.href || "/Journal/Article") + '"><img src="' + (item.image || "/images/aroma-kit.png") + '" alt="" /><span class="article-progress"><i></i></span></a>' +
                    '<div class="article-meta">' + escapeHtml(item.topic || "") + "<br />" + escapeHtml(item.date || "") + "</div>" +
                    '<h3><a href="' + (item.href || "/Journal/Article") + '">' + escapeHtml(item.title || "Article") + "</a></h3>" +
                    "<p>" + escapeHtml(item.lead || "") + "</p>" + stats;
            }
            journalRecentRow.appendChild(card);
            bindArticle(card);
        });
        journalRecent.hidden = seen.length === 0;
        syncAll();
    }

    document.querySelectorAll(".article-card, .journal-hero[data-id]").forEach(bindArticle);
    renderJournal();
    renderRecent();

    window.addEventListener("pageshow", renderRecent);
}

document.addEventListener("keydown", function (event) {
    if (event.key !== "Escape") return;
    closeFollow();
    closeEdit();
    closeAddress();
    closeCook();
    closeAbout();
    closeTopup();
});

(function () {
    var box = document.querySelector(".notify");
    if (!box) return;
    var items = box.querySelectorAll(".notify-item");
    var count = box.querySelector(".notify-count");
    var allTab = box.querySelector('.notify-tabs [data-kind="all"] span');
    var empty = box.querySelector(".notify-empty");
    var kind = "all";

    function refresh() {
        var fresh = box.querySelectorAll(".notify-item.is-new").length;
        var visible = 0;
        items.forEach(function (item) {
            item.hidden = kind !== "all" && item.getAttribute("data-kind") !== kind;
            if (!item.hidden) visible += 1;
        });
        count.textContent = fresh;
        count.hidden = fresh === 0;
        if (allTab) allTab.textContent = fresh || "";
        box.querySelector(".notify-btn").setAttribute("aria-label", fresh ? "Notifications, " + fresh + " new" : "Notifications");
        empty.hidden = visible > 0;
    }

    box.querySelectorAll(".notify-tabs button").forEach(function (button) {
        button.addEventListener("click", function () {
            kind = button.getAttribute("data-kind");
            box.querySelectorAll(".notify-tabs button").forEach(function (other) {
                other.classList.toggle("is-on", other === button);
            });
            refresh();
        });
    });

    items.forEach(function (item) {
        item.addEventListener("click", function () {
            if (!item.classList.contains("is-new")) return;
            item.classList.remove("is-new");
            var body = new URLSearchParams({ id: item.getAttribute("data-id"), __RequestVerificationToken: document.documentElement.getAttribute("data-csrf") || "" });
            fetch("/Notifications/Read", {
                method: "POST",
                keepalive: true,
                headers: { "Content-Type": "application/x-www-form-urlencoded", "X-Requested-With": "XMLHttpRequest" },
                body: body.toString()
            });
        });
    });

    box.querySelector(".notify-clear").addEventListener("click", function () {
        postArticle("/Notifications/ReadAll", {}).then(function (done) {
            if (done === null) return;
            box.querySelectorAll(".notify-item.is-new").forEach(function (item) {
                item.classList.remove("is-new");
            });
            document.querySelectorAll(".notice.is-new").forEach(function (item) {
                item.classList.remove("is-new");
            });
            refresh();
        });
    });

    refresh();
})();

document.querySelectorAll(".notice-read-all").forEach(function (button) {
    button.addEventListener("click", function () {
        postArticle("/Notifications/ReadAll", {}).then(function (done) {
            if (done === null) return;
            document.querySelectorAll(".notice.is-new, .notify-item.is-new").forEach(function (item) {
                item.classList.remove("is-new");
            });
            var count = document.querySelector(".notify-count");
            if (count) count.hidden = true;
            button.hidden = true;
        });
    });
});

function setCartCount(count) {
    var link = document.querySelector(".cart-link");
    if (!link) return;
    link.querySelector("span").textContent = count + " pcs";
    link.setAttribute("aria-label", "Cart, " + count + (count === 1 ? " item" : " items"));
    var badge = link.querySelector(".icon-count");
    if (badge) {
        if (String(count) !== badge.textContent) {
            badge.classList.remove("is-bump");
            void badge.offsetWidth;
            badge.classList.add("is-bump");
        }
        badge.textContent = count;
        badge.hidden = count === 0;
    }
}

function replaceMiniCart(html) {
    var panel = document.querySelector(".cart-drop .mini-cart");
    if (!panel) return;
    var holder = document.createElement("div");
    holder.innerHTML = html.trim();
    var next = holder.querySelector(".mini-cart");
    if (!next) return;
    panel.replaceWith(next);
    setCartCount(parseInt(next.getAttribute("data-count"), 10) || 0);
}

function refreshMiniCart() {
    fetch("/Cart/Panel", { headers: { "X-Requested-With": "XMLHttpRequest" } })
        .then(function (response) { return response.ok ? response.text() : null; })
        .then(function (html) { if (html) replaceMiniCart(html); })
        .catch(function () { });
}

document.addEventListener("click", function (event) {
    var qty = event.target.closest("[data-mini-qty]");
    var remove = event.target.closest("[data-mini-remove]");
    if (!qty && !remove) return;

    var button = qty || remove;
    var body = new URLSearchParams();
    body.set("__RequestVerificationToken", document.documentElement.getAttribute("data-csrf") || "");
    var url;
    if (qty) {
        url = "/Cart/Update";
        body.set("id", qty.getAttribute("data-mini-qty"));
        body.set("quantity", qty.getAttribute("data-quantity"));
    } else {
        url = "/Cart/Remove";
        body.set("id", remove.getAttribute("data-mini-remove"));
    }

    button.disabled = true;
    fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/x-www-form-urlencoded",
            "X-Requested-With": "XMLHttpRequest"
        },
        body: body.toString()
    }).then(function (response) {
        if (response.status === 401) {
            window.location.href = "/Account/Login";
            return;
        }
        if (!response.ok) {
            return response.json().then(function (payload) {
                showToast((payload.errors || []).join(" ") || "Could not change the cart.");
                button.disabled = false;
            });
        }
        return response.text().then(function (html) {
            replaceMiniCart(html);
            if (remove) showToast("Removed from cart");
            if (window.location.pathname.toLowerCase().indexOf("/cart") === 0) window.location.reload();
        });
    }).catch(function () {
        button.disabled = false;
        showToast("The API is not running.");
    });
});

document.querySelectorAll("[data-toggle-address]").forEach(function (button) {
    button.addEventListener("click", function () {
        var form = document.querySelector("[data-address-form]");
        if (!form) return;
        form.hidden = !form.hidden;
        if (!form.hidden) form.querySelector("input[name=line]").focus();
    });
});

document.querySelectorAll("#checkout input[name=addressId]").forEach(function (radio) {
    radio.addEventListener("change", function () {
        var box = document.querySelector("[data-recipient]");
        if (!box) return;
        box.hidden = false;
        box.querySelector("[data-recipient-name]").textContent = radio.getAttribute("data-name");
        box.querySelector("[data-recipient-phone]").textContent = radio.getAttribute("data-phone");
    });
});

var compareKey = "janz-compare-ids";

function readCompare() {
    try {
        var list = JSON.parse(localStorage.getItem(compareKey) || "[]");
        return Array.isArray(list) ? list.filter(function (id) { return typeof id === "number"; }) : [];
    } catch (error) {
        return [];
    }
}

function writeCompare(list) {
    try { localStorage.setItem(compareKey, JSON.stringify(list)); } catch (error) { }
    showCompareCount(true);
    markCompareButtons();
}

function showCompareCount(bump) {
    var badge = document.querySelector("[data-compare-count]");
    if (!badge) return;
    var count = readCompare().length;
    if (bump && String(count) !== badge.textContent) {
        badge.classList.remove("is-bump");
        void badge.offsetWidth;
        badge.classList.add("is-bump");
    }
    badge.textContent = count;
    badge.hidden = count === 0;
}

function markCompareButtons() {
    var list = readCompare();
    document.querySelectorAll("[data-compare-toggle]").forEach(function (button) {
        var on = list.indexOf(parseInt(button.getAttribute("data-compare-toggle"), 10)) !== -1;
        button.classList.toggle("is-on", on);
        button.setAttribute("aria-pressed", on ? "true" : "false");
        button.setAttribute("aria-label", on ? "Remove from compare" : "Compare");
    });
}

document.addEventListener("click", function (event) {
    var button = event.target.closest("[data-compare-toggle]");
    if (!button) return;
    var id = parseInt(button.getAttribute("data-compare-toggle"), 10);
    var list = readCompare();
    var index = list.indexOf(id);
    if (index === -1) list.push(id);
    else list.splice(index, 1);
    writeCompare(list);
    showToast(index === -1 ? "Added to compare" : "Removed from compare");
});

showCompareCount(false);
markCompareButtons();

(function () {
    var table = document.querySelector(".compare-table");
    var source = document.getElementById("compare-products");
    if (!table || !source) return;

    var products = JSON.parse(source.textContent || "[]");
    var diffBox = document.querySelector(".compare-diff input");
    var modal = document.querySelector(".compare-modal");
    var pick = modal.querySelector(".compare-pick");
    var search = modal.querySelector(".compare-search input");
    var pickEmpty = modal.querySelector(".compare-pick-empty");

    var rows = [
        { label: "Category", value: function (item) { return item.category; } },
        { label: "Price", value: function (item) { return "₼" + item.price.toFixed(2); } },
        { label: "Availability", value: function (item) { return item.stock > 0 ? item.stock + " in stock" : "Out of stock"; } },
        { label: "Rating", value: function (item) { return item.rating ? item.rating.toFixed(1) + " (" + item.reviews + ")" : "-"; } },
        { label: "Roast", value: function (item) { return item.roast || "-"; } },
        { label: "Processing", value: function (item) { return item.processing || "-"; } },
        { label: "Region", value: function (item) { return item.region || "-"; } },
        { label: "Taste notes", value: function (item) { return (item.notes || []).join(", ") || "-"; } },
        { label: "Acidity", value: function (item) { return item.acidity != null ? item.acidity + "%" : "-"; } },
        { label: "Density", value: function (item) { return item.density != null ? item.density + "%" : "-"; } },
        { label: "Tags", value: function (item) { return item.tags.join(", ") || "-"; }, tags: true },
        { label: "Description", value: function (item) { return item.description || "-"; } }
    ];

    var trashIcon = '<svg viewBox="0 0 24 24" aria-hidden="true"><path d="M5 7h14M10 7V5h4v2M7 7l1 13h8l1-13M10.5 11v5.5M13.5 11v5.5" fill="none" stroke="currentColor" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round"/></svg>';
    var bagIcon = '<svg viewBox="0 0 24 24" aria-hidden="true"><path d="M4.5 8h15v11.5a1.5 1.5 0 0 1-1.5 1.5H6a1.5 1.5 0 0 1-1.5-1.5z" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linejoin="round"/><path d="M8.5 8V6.5a3.5 3.5 0 0 1 7 0V8M9.5 13l2 2 3.5-3.5" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round"/></svg>';
    var plusIcon = '<svg viewBox="0 0 24 24" aria-hidden="true"><path d="M12 4v16M4 12h16" fill="none" stroke="currentColor" stroke-width="1.6" stroke-linecap="round"/></svg>';

    function el(tag, className, text) {
        var node = document.createElement(tag);
        if (className) node.className = className;
        if (text !== undefined) node.textContent = text;
        return node;
    }

    function findProduct(id) {
        return products.filter(function (item) { return item.id === id; })[0];
    }

    function render() {
        var chosen = readCompare().map(findProduct).filter(Boolean);
        table.innerHTML = "";
        table.style.setProperty("--compare-cols", chosen.length + 1);
        table.classList.toggle("is-empty", !chosen.length);
        diffBox.disabled = chosen.length < 2;

        var head = el("div", "compare-row compare-head");
        head.appendChild(el("div", "compare-label"));
        chosen.forEach(function (item) {
            var card = el("div", "compare-card");
            var top = el("div", "compare-card-top");
            var name = el("a", "compare-name", item.name);
            name.href = "/Products/Detail/" + item.id;
            top.appendChild(name);
            var remove = el("button", "compare-remove");
            remove.type = "button";
            remove.setAttribute("aria-label", "Remove " + item.name + " from compare");
            remove.innerHTML = trashIcon;
            remove.addEventListener("click", function () {
                writeCompare(readCompare().filter(function (id) { return id !== item.id; }));
                render();
                showToast("Removed from compare");
            });
            top.appendChild(remove);
            card.appendChild(top);

            var photo = el("a", "compare-photo");
            photo.href = "/Products/Detail/" + item.id;
            var img = el("img");
            img.src = item.photo;
            img.alt = item.name;
            photo.appendChild(img);
            card.appendChild(photo);

            var buy = el("button", "compare-buy");
            buy.type = "button";
            buy.setAttribute("data-cart-add", item.id);
            if (item.variant) buy.setAttribute("data-variant", item.variant);
            buy.setAttribute("aria-label", "Add " + item.name + " to cart");
            buy.disabled = item.stock <= 0;
            buy.innerHTML = "<strong>₼" + item.price.toFixed(2) + "</strong>" + bagIcon;
            card.appendChild(buy);
            head.appendChild(card);
        });

        var addCell = el("div", "compare-add-cell");
        var add = el("button", "compare-add");
        add.type = "button";
        add.setAttribute("aria-label", "Add products to compare");
        add.innerHTML = plusIcon;
        add.addEventListener("click", openModal);
        addCell.appendChild(add);
        head.appendChild(addCell);
        table.appendChild(head);

        if (!chosen.length) return;

        rows.forEach(function (rowInfo) {
            var row = el("div", "compare-row");
            var values = chosen.map(rowInfo.value);
            row.dataset.same = values.every(function (value) { return value === values[0]; }) ? "true" : "false";
            row.appendChild(el("div", "compare-label", rowInfo.label));
            chosen.forEach(function (item, index) {
                var cell = el("div", "compare-cell");
                if (rowInfo.tags && item.tags.length) {
                    var tags = el("div", "compare-tags");
                    item.tags.forEach(function (tag) { tags.appendChild(el("span", "", tag)); });
                    cell.appendChild(tags);
                } else {
                    cell.textContent = values[index];
                }
                row.appendChild(cell);
            });
            row.appendChild(el("div"));
            table.appendChild(row);
        });
        applyDiff();
    }

    function applyDiff() {
        var only = diffBox.checked && !diffBox.disabled;
        table.querySelectorAll(".compare-row[data-same]").forEach(function (row) {
            row.hidden = only && row.dataset.same === "true";
        });
    }

    diffBox.addEventListener("change", applyDiff);

    function fillPick() {
        var chosen = readCompare();
        pick.innerHTML = "";
        products.forEach(function (item) {
            var label = el("label", "compare-pick-item");
            label.dataset.name = item.name.toLowerCase();
            var box = el("input");
            box.type = "checkbox";
            box.value = item.id;
            box.checked = chosen.indexOf(item.id) !== -1;
            label.appendChild(box);
            var img = el("img");
            img.src = item.photo;
            img.alt = "";
            label.appendChild(img);
            label.appendChild(el("span", "", item.name));
            pick.appendChild(label);
        });
    }

    function filterPick() {
        var text = search.value.trim().toLowerCase();
        var shown = 0;
        pick.querySelectorAll(".compare-pick-item").forEach(function (item) {
            item.hidden = text !== "" && item.dataset.name.indexOf(text) === -1;
            if (!item.hidden) shown += 1;
        });
        pickEmpty.hidden = shown > 0;
    }

    var lastFocus = null;

    function openModal() {
        lastFocus = document.activeElement;
        fillPick();
        search.value = "";
        filterPick();
        modal.hidden = false;
        document.body.classList.add("is-locked");
        search.focus();
    }

    function closeModal() {
        modal.hidden = true;
        document.body.classList.remove("is-locked");
        if (lastFocus) lastFocus.focus();
    }

    search.addEventListener("input", filterPick);
    modal.querySelector(".compare-modal-close").addEventListener("click", closeModal);
    modal.addEventListener("click", function (event) {
        if (event.target === modal) closeModal();
    });
    document.addEventListener("keydown", function (event) {
        if (event.key === "Escape" && !modal.hidden) closeModal();
    });

    modal.querySelector(".compare-confirm").addEventListener("click", function () {
        var picked = [];
        pick.querySelectorAll("input:checked").forEach(function (box) { picked.push(parseInt(box.value, 10)); });
        var list = readCompare().filter(function (id) { return picked.indexOf(id) !== -1; });
        picked.forEach(function (id) { if (list.indexOf(id) === -1) list.push(id); });
        writeCompare(list);
        closeModal();
        render();
    });

    render();
})();

(function () {
    var page = document.getElementById("user-profile");
    if (!page) return;

    var colorCount = 9;
    var svgNs = "http://www.w3.org/2000/svg";

    page.querySelectorAll(".up-pref").forEach(function (pref) {
        var items = pref.querySelectorAll("li");
        var svg = pref.querySelector(".up-donut");
        var total = 0;
        items.forEach(function (item) { total += parseFloat(item.getAttribute("data-value")) || 0; });
        var radius = 15.915;
        var offset = 25;

        var wrap = document.createElement("div");
        wrap.className = "up-donut-wrap";
        svg.parentNode.insertBefore(wrap, svg);
        wrap.appendChild(svg);
        var tip = document.createElement("div");
        tip.className = "up-tip";
        tip.hidden = true;
        wrap.appendChild(tip);

        function showPart(index, event) {
            var item = items[index];
            var rings = svg.querySelectorAll("circle");
            rings.forEach(function (ring, ringIndex) { ring.classList.toggle("is-on", ringIndex === index); });
            items.forEach(function (other, otherIndex) { other.classList.toggle("is-on", otherIndex === index); });
            tip.textContent = item.firstChild.textContent.trim() + " ";
            tip.appendChild(item.querySelector("b").cloneNode(true));
            tip.hidden = false;
            var box = wrap.getBoundingClientRect();
            var x = event && event.clientX ? event.clientX - box.left : box.width / 2;
            var y = event && event.clientY ? event.clientY - box.top : box.height / 2;
            tip.style.left = x + "px";
            tip.style.top = y + "px";
        }

        function hidePart() {
            tip.hidden = true;
            svg.querySelectorAll("circle").forEach(function (ring) { ring.classList.remove("is-on"); });
            items.forEach(function (item) { item.classList.remove("is-on"); });
        }

        wrap.addEventListener("mouseleave", hidePart);
        items.forEach(function (item, index) {
            var value = parseFloat(item.getAttribute("data-value")) || 0;
            var share = total ? value / total * 100 : 0;
            var color = "var(--up-c" + (index % colorCount) + ")";

            var mark = document.createElement("b");
            mark.textContent = Math.round(share) + "%";
            item.appendChild(document.createTextNode(" "));
            item.appendChild(mark);
            item.style.setProperty("--dot", color);

            var ring = document.createElementNS(svgNs, "circle");
            ring.setAttribute("cx", "21");
            ring.setAttribute("cy", "21");
            ring.setAttribute("r", String(radius));
            ring.setAttribute("fill", "none");
            ring.style.stroke = color;
            ring.setAttribute("stroke-width", "4.5");
            ring.setAttribute("stroke-dasharray", Math.max(share - 1, 0.5) + " " + (100 - Math.max(share - 1, 0.5)));
            ring.setAttribute("stroke-dashoffset", String(offset));
            svg.appendChild(ring);
            offset -= share;

            ring.addEventListener("mousemove", function (event) { showPart(index, event); });
            item.addEventListener("mouseenter", function () { showPart(index, null); });
            item.addEventListener("mouseleave", hidePart);
        });
    });

    page.querySelectorAll(".up-tabs button").forEach(function (tab) {
        tab.addEventListener("click", function () {
            var name = tab.getAttribute("data-pref");
            page.querySelectorAll(".up-tabs button").forEach(function (other) {
                other.classList.toggle("is-on", other === tab);
                other.setAttribute("aria-selected", other === tab ? "true" : "false");
            });
            page.querySelectorAll(".up-pref").forEach(function (pref) {
                pref.hidden = pref.getAttribute("data-pref") !== name;
            });
        });
    });

    var follow = page.querySelector(".up-follow");
    var followers = page.querySelector("[data-followers]");
    var postFollows = page.querySelectorAll(".feed-follow");
    var profileId = page.getAttribute("data-user-id");

    function setFollow(on) {
        if (follow) {
            follow.classList.toggle("is-on", on);
            follow.setAttribute("aria-pressed", on ? "true" : "false");
            follow.textContent = on ? "Following" : "Follow";
        }
        postFollows.forEach(function (button) {
            button.classList.toggle("is-on", on);
            button.setAttribute("aria-pressed", on ? "true" : "false");
            button.textContent = on ? "Following" : "Follow";
        });
    }

    if (follow) {
        follow.addEventListener("click", function () {
            if (!isSignedIn()) {
                openAuth();
                return;
            }
            var on = !follow.classList.contains("is-on");
            postArticle("/Community/Follow", { userId: profileId, on: on }).then(function (result) {
                if (!result) return;
                followers.textContent = String(Math.max(0, (parseInt(followers.textContent, 10) || 0) + (on ? 1 : -1)));
                setFollow(on);
            });
        });
    }

    if (follow && postFollows.length) {
        new MutationObserver(function () {
            var on = postFollows[0].classList.contains("is-on");
            if (on === follow.classList.contains("is-on")) return;
            followers.textContent = String(Math.max(0, (parseInt(followers.textContent, 10) || 0) + (on ? 1 : -1)));
            setFollow(on);
        }).observe(postFollows[0], { attributes: true, attributeFilter: ["class"] });
    }

    page.querySelector(".up-share").addEventListener("click", function () {
        if (typeof openShare === "function") openShare(window.location.href);
    });
})();

document.querySelectorAll("[data-tabs]").forEach(function (tabs) {
    tabs.addEventListener("click", function (event) {
        var tab = event.target.closest("[data-tab]");
        if (!tab) return;
        tabs.querySelectorAll("[data-tab]").forEach(function (item) {
            item.classList.toggle("is-on", item === tab);
        });
        document.querySelectorAll("[data-tab-panel]").forEach(function (panel) {
            panel.hidden = panel.getAttribute("data-tab-panel") !== tab.getAttribute("data-tab");
        });
    });
});

document.querySelectorAll("[data-show-tab]").forEach(function (button) {
    button.addEventListener("click", function () {
        var tab = document.querySelector('[data-tab="' + button.getAttribute("data-show-tab") + '"]');
        if (!tab) return;
        tab.click();
        tab.scrollIntoView({ behavior: "smooth", block: "start" });
    });
});

document.querySelectorAll('a[href^="?tab=comments"]').forEach(function (link) {
    link.addEventListener("click", function (event) {
        var tab = document.querySelector('[data-tab="comments"]');
        if (!tab) return;
        event.preventDefault();
        tab.click();
        document.getElementById("comments").scrollIntoView({ behavior: "smooth", block: "start" });
    });
});

document.querySelectorAll("form[data-auto-submit]").forEach(function (form) {
    form.addEventListener("change", function (event) {
        if (event.target.matches("input[type=checkbox], input[type=radio], select, input[type=number]")) form.requestSubmit();
    });
});

var catalogSection = document.querySelector("#catalog, #tea, #chocolate, #bundles");
var catalogBoard = catalogSection ? catalogSection.querySelector(".catalog-board") : null;
var catalogTicket = 0;

function catalogUrl(form, submitter) {
    var params = new URLSearchParams();
    new FormData(form).forEach(function (value, name) {
        if (value !== "") params.append(name, value);
    });
    if (submitter && submitter.name) params.set(submitter.name, submitter.value);
    var text = params.toString();
    return location.pathname + (text ? "?" + text : "");
}

function syncCatalog(fresh) {
    var inputs = catalogBoard.querySelectorAll(".filter-col input");
    fresh.querySelectorAll(".filter-col input").forEach(function (item, index) {
        var current = inputs[index];
        if (!current) return;
        if (current.type === "checkbox" || current.type === "radio") current.checked = item.checked;
        else current.value = item.value;
    });
    catalogBoard.querySelectorAll(":scope > input[type=hidden]").forEach(function (item) {
        item.remove();
    });
    fresh.querySelectorAll(":scope > input[type=hidden]").forEach(function (item) {
        catalogBoard.insertBefore(item, catalogBoard.firstChild);
    });
    var links = catalogSection.querySelectorAll(".catalog-tile, .catalog-tags a");
    fresh.ownerDocument.querySelectorAll("#" + catalogSection.id + " .catalog-tile, #" + catalogSection.id + " .catalog-tags a").forEach(function (item, index) {
        if (links[index]) links[index].classList.toggle("is-on", item.classList.contains("is-on"));
    });
}

function loadCatalog(url, push) {
    var ticket = ++catalogTicket;
    catalogBoard.classList.add("is-loading");
    fetch(url, { headers: { "X-Requested-With": "XMLHttpRequest" } })
        .then(function (response) { return response.text(); })
        .then(function (html) {
            if (ticket !== catalogTicket) return;
            var doc = new DOMParser().parseFromString(html, "text/html");
            var fresh = doc.querySelector("#" + catalogSection.id + " .catalog-board");
            if (!fresh) {
                location.href = url;
                return;
            }
            var main = fresh.querySelector(".listing-main");
            catalogBoard.querySelector(".listing-main").replaceWith(main);
            syncCatalog(fresh);
            if (push) history.pushState(null, "", url);
            bindGrind(main);
            bindBuyRows(main);
            markCompareButtons();
            catalogBoard.classList.remove("is-loading");
        })
        .catch(function () {
            location.href = url;
        });
}

if (catalogBoard) {
    catalogBoard.addEventListener("submit", function (event) {
        event.preventDefault();
        loadCatalog(catalogUrl(catalogBoard, event.submitter), true);
    });

    catalogSection.addEventListener("click", function (event) {
        var link = event.target.closest(".catalog-tile, .catalog-tags a, .catalog-empty a");
        if (!link || event.metaKey || event.ctrlKey || event.shiftKey || event.button !== 0) return;
        var href = link.getAttribute("href");
        if (!href || href === "#") return;
        event.preventDefault();
        loadCatalog(href.charAt(0) === "?" ? location.pathname + href : href, true);
    });

    window.addEventListener("popstate", function () {
        loadCatalog(location.pathname + location.search, false);
    });
}

document.querySelectorAll("[data-copy]").forEach(function (button) {
    button.addEventListener("click", function () {
        var text = button.getAttribute("data-copy");
        if (navigator.clipboard) {
            navigator.clipboard.writeText(text).then(function () {
                showToast("Copied: " + text);
            });
        }
    });
});

(function () {
    var box = document.querySelector(".assistant");
    if (!box) return;

    var toggle = box.querySelector(".assistant-toggle");
    var panel = box.querySelector(".assistant-panel");
    var log = box.querySelector(".assistant-log");
    var chips = box.querySelector(".assistant-chips");
    var form = box.querySelector(".assistant-form");
    var input = form.querySelector("input");
    var catalog = box.getAttribute("data-catalog") || "catalog.html";
    var started = false;

    var brews = [
        { label: "Espresso machine", query: "use=espresso" },
        { label: "Moka pot", query: "use=espresso" },
        { label: "Filter or pour over", query: "use=filter" },
        { label: "French press", query: "use=filter" },
        { label: "Turkish coffee", query: "use=turkish" },
        { label: "With milk", query: "use=milk" },
        { label: "Capsules", query: "format=capsules" }
    ];

    var tastes = [
        { label: "Chocolate and nuts", query: "taste=chocolate" },
        { label: "Flowers and citrus", query: "taste=flowers" },
        { label: "Fruits and berries", query: "taste=fruit" },
        { label: "Something unusual", query: "taste=alcohol" },
        { label: "Anything", query: "" }
    ];

    var answers = [
        {
            words: ["deliver", "shipping", "courier", "ship", "when will"],
            text: "Janz courier delivers all over Baku in 2–3 days, free of charge. Regions get their orders by Azerpost."
        },
        {
            words: ["pay", "card", "cash", "stripe"],
            text: "You can pay by card online on the order page right after placing the order, or pay cash or card to the courier."
        },
        {
            words: ["promo", "coupon", "discount", "code", "sale", "cheaper"],
            text: "Type a promo code in the cart and press Apply, the discount shows up in the total. New customers get a code for the first order in their account."
        },
        {
            words: ["status", "track", "where is my order", "my order"],
            text: "Open your account and go to Orders: every order shows its status there."
        },
        {
            words: ["return", "refund", "exchange", "broken", "wrong"],
            text: "Something went wrong with an order? Write to us on <a href=\"https://wa.me/994702175557\">WhatsApp</a> and we will sort it out."
        },
        {
            words: ["grind", "brew", "recipe", "how to make", "how much coffee"],
            text: "A good start: espresso 18 g in, 36 g out in 25–30 s; filter 15 g per 250 ml of water at 92–94 °C. Choose the grind on the product page."
        },
        {
            words: ["contact", "phone", "address", "call", "where are you", "whatsapp"],
            text: "Call us at <a href=\"tel:+994702175557\">+994 70 217 55 57</a> or write on <a href=\"https://wa.me/994702175557\">WhatsApp</a>. We are in Nesimi, Masud Davudoglu 171."
        },
        {
            words: ["hello", "hi", "salam", "hey"],
            text: "Hi! I can pick a coffee for you or answer questions about delivery, payment and promo codes."
        }
    ];

    function say(html, mine) {
        var line = document.createElement("div");
        if (mine) {
            line.className = "is-mine";
            line.textContent = html;
        } else {
            line.innerHTML = html;
        }
        log.appendChild(line);
        log.scrollTop = log.scrollHeight;
    }

    function offer(list, onPick) {
        chips.innerHTML = "";
        list.forEach(function (item) {
            var button = document.createElement("button");
            button.type = "button";
            button.textContent = item.label;
            button.addEventListener("click", function () {
                say(item.label, true);
                onPick(item);
            });
            chips.appendChild(button);
        });
    }

    function menu() {
        offer([
            { label: "Pick a coffee" },
            { label: "Delivery" },
            { label: "Payment" },
            { label: "Promo codes" },
            { label: "Contacts" }
        ], function (item) {
            if (item.label === "Pick a coffee") startPick();
            else reply(item.label);
        });
    }

    function startPick() {
        say("How do you make your coffee?");
        offer(brews, function (brew) {
            say("And what taste do you like?");
            offer(tastes, function (taste) {
                var query = [brew.query, taste.query].filter(Boolean).join("&");
                say("Here are the coffees for you: <a href=\"" + catalog + "?" + query + "\">show coffees</a>. Anything else?");
                menu();
            });
        });
    }

    function reply(question) {
        var text = question.toLowerCase();
        if (text.indexOf("pick") >= 0 || text.indexOf("choose") >= 0 || text.indexOf("recommend") >= 0) {
            startPick();
            return;
        }
        var found = answers.filter(function (answer) {
            return answer.words.some(function (word) { return text.indexOf(word) >= 0; });
        })[0];
        say(found ? found.text : "I am not sure I got it. Try one of the topics below or write to us on <a href=\"https://wa.me/994702175557\">WhatsApp</a>.");
        menu();
    }

    function open() {
        panel.hidden = false;
        box.classList.add("is-open");
        toggle.setAttribute("aria-expanded", "true");
        if (!started) {
            started = true;
            say("Hi! I am the Janz assistant. I can pick a coffee for you or answer questions.");
            menu();
        }
        input.focus();
    }

    function close() {
        panel.hidden = true;
        box.classList.remove("is-open");
        toggle.setAttribute("aria-expanded", "false");
        toggle.focus();
    }

    toggle.addEventListener("click", open);
    box.querySelector(".assistant-close").addEventListener("click", close);

    document.addEventListener("keydown", function (event) {
        if (event.key === "Escape" && !panel.hidden) close();
    });

    form.addEventListener("submit", function (event) {
        event.preventDefault();
        var question = input.value.trim();
        if (!question) return;
        input.value = "";
        say(question, true);
        var text = question.toLowerCase();
        if (text.indexOf("pick") >= 0 || text.indexOf("choose") >= 0 || text.indexOf("recommend") >= 0) {
            reply(question);
            return;
        }
        var wait = document.createElement("div");
        wait.textContent = "...";
        log.appendChild(wait);
        log.scrollTop = log.scrollHeight;
        postArticle("/Assistant/Ask", { question: question }).then(function (data) {
            wait.remove();
            if (data && data.answer) {
                say(data.answer.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/\n/g, "<br>"));
                menu();
            } else {
                reply(question);
            }
        });
    });
})();

// Reviews page: ticking a star filter reloads the list with that filter
document.addEventListener("change", function (event) {
    var box = event.target;
    if (box instanceof HTMLInputElement && box.type === "checkbox" && box.hasAttribute("data-auto-submit") && box.form) {
        box.form.submit();
    }
});
