(function () {
    var root = document.documentElement;
    if (localStorage.getItem("janz-theme") === "day") root.classList.add("theme-day");
})();

var authOverlay = document.createElement("div");
authOverlay.className = "auth-overlay";
authOverlay.innerHTML =
    '<div class="auth-modal" role="dialog" aria-labelledby="auth-title" aria-modal="true">' +
        "<h2 id=\"auth-title\">Sign in</h2>" +
        '<form action="/Home/SignIn" method="post">' +
            '<p class="entry-error" hidden></p>' +
            '<label class="entry-field"><input type="email" name="email" placeholder="Email" autocomplete="email" required /></label>' +
            '<label class="entry-field"><input type="password" name="password" placeholder="Password" autocomplete="current-password" required /><button type="button" class="entry-eye" aria-label="Show password"><svg viewBox="0 0 24 24" aria-hidden="true"><path d="M3 12s3.6-6.5 9-6.5S21 12 21 12s-3.6 6.5-9 6.5S3 12 3 12z" fill="none" stroke="currentColor" stroke-width="1.6"/><circle cx="12" cy="12" r="2.4" fill="none" stroke="currentColor" stroke-width="1.6"/></svg></button></label>' +
            '<button type="submit" class="account-signin">Sign in</button>' +
            '<a class="entry-forgot" href="#">Forgot password?</a>' +
            '<label class="entry-remember"><input type="checkbox" name="remember" /> Remember password</label>' +
        "</form>" +
    "</div>";

function isSignedIn() {
    return document.documentElement.getAttribute("data-auth") === "1";
}

function needsAuth(target) {
    return target.closest(
        'a[href="/Home/Wishlist"], [aria-label="Add to wishlist"], [aria-label="Likes"], [aria-label="Save"], .article-comment-votes button'
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
    fetch("/Home/SignIn", {
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
    var signLink = event.target.closest('a[href="/Home/SignIn"]');
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

document.querySelectorAll("#register .entry-eye, #signin .entry-eye").forEach(function (button) {
    button.addEventListener("click", function () {
        var field = button.previousElementSibling;
        var hidden = field.type === "password";
        field.type = hidden ? "text" : "password";
        button.setAttribute("aria-label", hidden ? "Hide password" : "Show password");
    });
});

var registerForm = document.querySelector("#register form");
if (registerForm) {
    registerForm.addEventListener("submit", function (event) {
        var password = this.querySelector("[name='password']");
        var repeat = this.querySelector("[name='password2']");
        if (password.value !== repeat.value) {
            event.preventDefault();
            repeat.setCustomValidity("Passwords do not match");
            repeat.reportValidity();
        } else {
            repeat.setCustomValidity("");
        }
    });
}

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
    form.addEventListener("submit", function (event) {
        event.preventDefault();
        var where = form.getAttribute("data-where") || "/Home/Catalog";
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

document.querySelectorAll(".shot").forEach(function (card) {
    var video = card.querySelector("video");
    if (!video) return;
    card.addEventListener("mouseenter", function () {
        video.play();
    });
    card.addEventListener("mouseleave", function () {
        video.pause();
        video.currentTime = 0;
    });
});

document.querySelectorAll(".grind").forEach(function (menu) {
    var label = menu.querySelector("summary span");
    menu.querySelectorAll(".grind-list button").forEach(function (option) {
        option.addEventListener("click", function () {
            menu.querySelectorAll(".grind-list button").forEach(function (item) {
                item.classList.toggle("is-on", item === option);
            });
            label.textContent = option.textContent;
            menu.removeAttribute("open");
        });
    });
});

document.querySelectorAll(".buy-row").forEach(function (row) {
    var weights = row.querySelectorAll(".weight");
    weights.forEach(function (weight) {
        weight.addEventListener("click", function () {
            weights.forEach(function (item) {
                item.classList.toggle("is-on", item === weight);
            });
        });
    });
});

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
