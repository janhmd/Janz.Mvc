var adminTheme = document.querySelector(".admin-theme");
if (adminTheme) {
    adminTheme.addEventListener("click", function () {
        var day = document.documentElement.classList.toggle("theme-day");
        try { localStorage.setItem("janz-theme", day ? "day" : "night"); } catch (error) { }
    });
}

document.querySelectorAll("form[data-confirm]").forEach(function (form) {
    form.addEventListener("submit", function (event) {
        if (!confirm(form.getAttribute("data-confirm"))) {
            event.preventDefault();
        }
    });
});

document.querySelectorAll(".admin-flash").forEach(function (flash) {
    setTimeout(function () { flash.classList.add("is-hidden"); }, 5000);
});

document.querySelectorAll("[data-preview-input]").forEach(function (input) {
    var box = document.querySelector('[data-preview="' + input.getAttribute("data-preview-input") + '"]');
    if (!box) return;
    var single = !input.multiple;
    var original = box.innerHTML;

    input.addEventListener("change", function () {
        box.innerHTML = single && input.files.length === 0 ? original : "";
        Array.prototype.forEach.call(input.files, function (file) {
            var figure = document.createElement("figure");
            figure.className = "admin-thumb is-new" + (single ? " is-main" : "");
            var image = document.createElement("img");
            image.alt = "";
            image.src = URL.createObjectURL(file);
            var caption = document.createElement("figcaption");
            var label = document.createElement("span");
            label.className = "admin-thumb-label";
            label.textContent = file.size > 2 * 1024 * 1024 ? "Larger than 2 MB" : (single ? "New main" : "New");
            if (file.size > 2 * 1024 * 1024) figure.classList.add("is-invalid");
            caption.appendChild(label);
            figure.appendChild(image);
            figure.appendChild(caption);
            box.appendChild(figure);
        });
    });
});

document.querySelectorAll("[data-remove-toggle]").forEach(function (box) {
    box.addEventListener("change", function () {
        var figure = box.closest(".admin-thumb");
        figure.classList.toggle("is-removed", box.checked);
        box.parentElement.querySelector("span").textContent = box.checked ? "Undo" : "Remove";
    });
});

(function () {
    var list = document.querySelector("[data-variant-list]");
    if (!list) return;
    var template = document.querySelector("[data-variant-template]");
    var addButton = document.querySelector("[data-variant-add]");
    var count = document.querySelector("[data-variant-count]");
    var note = document.querySelector("[data-variant-note]");
    var priceFields = document.querySelectorAll("[data-price-fields] input");
    var max = parseInt(list.getAttribute("data-max"), 10) || 8;

    function refresh() {
        var rows = list.querySelectorAll("[data-variant-row]");
        rows.forEach(function (row, index) {
            row.querySelectorAll("[name]").forEach(function (field) {
                field.name = field.name.replace(/^Variants\[[^\]]+\]/, "Variants[" + index + "]");
            });
            row.querySelector("[data-variant-title]").textContent = "Variant " + (index + 1);
        });
        if (count) count.textContent = rows.length + " / " + max;
        addButton.disabled = rows.length >= max;
        var hasVariants = rows.length > 0;
        priceFields.forEach(function (field) { field.disabled = hasVariants; });
        if (note) note.hidden = !hasVariants;
    }

    addButton.addEventListener("click", function () {
        var row = template.content.firstElementChild.cloneNode(true);
        list.appendChild(row);
        refresh();
        row.querySelector("input[name$='.Format']").focus();
    });

    list.addEventListener("click", function (event) {
        var remove = event.target.closest("[data-variant-remove]");
        if (!remove) return;
        remove.closest("[data-variant-row]").remove();
        refresh();
    });

    list.addEventListener("change", function (event) {
        var keep = event.target.closest("[data-keep-toggle]");
        if (keep) {
            keep.closest(".admin-thumb").classList.toggle("is-removed", !keep.checked);
            keep.parentElement.querySelector("span").textContent = keep.checked ? "Remove" : "Undo";
            return;
        }
        var input = event.target.closest("[data-variant-input]");
        if (!input) return;
        var box = input.closest("[data-variant-row]").querySelector("[data-variant-preview]");
        box.innerHTML = "";
        Array.prototype.forEach.call(input.files, function (file) {
            var figure = document.createElement("figure");
            figure.className = "admin-thumb is-new" + (file.size > 2 * 1024 * 1024 ? " is-invalid" : "");
            var image = document.createElement("img");
            image.alt = "";
            image.src = URL.createObjectURL(file);
            var caption = document.createElement("figcaption");
            var label = document.createElement("span");
            label.className = "admin-thumb-label";
            label.textContent = file.size > 2 * 1024 * 1024 ? "Larger than 2 MB" : "New";
            caption.appendChild(label);
            figure.appendChild(image);
            figure.appendChild(caption);
            box.appendChild(figure);
        });
    });

    refresh();
})();

document.querySelectorAll("input[data-upper]").forEach(function (input) {
    input.addEventListener("input", function () {
        input.value = input.value.toUpperCase().replace(/[^A-Z0-9]/g, "");
    });
});

document.querySelectorAll("[data-generate]").forEach(function (button) {
    button.addEventListener("click", function () {
        var letters = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var code = "JANZ";
        for (var i = 0; i < 6; i += 1) code += letters[Math.floor(Math.random() * letters.length)];
        button.parentNode.querySelector("input").value = code;
    });
});

(function () {
    var body = document.querySelector("[data-level-rows]");
    if (!body) return;
    var template = document.querySelector("[data-level-template]");

    function renumber() {
        body.querySelectorAll("[data-level-row]").forEach(function (row, index) {
            row.querySelectorAll("[name]").forEach(function (input) {
                input.name = input.name.replace(/Levels\[[^\]]*\]/, "Levels[" + index + "]");
            });
        });
    }

    body.addEventListener("click", function (event) {
        var button = event.target.closest("[data-level-remove]");
        if (!button) return;
        button.closest("[data-level-row]").remove();
        renumber();
    });

    document.querySelector("[data-add-level]").addEventListener("click", function () {
        body.insertAdjacentHTML("beforeend", template.innerHTML);
        renumber();
        body.lastElementChild.querySelector("input:not([type=hidden])").focus();
    });
})();

(function () {
    var box = document.querySelector(".admin-assistant");
    if (!box) return;

    var toggle = box.querySelector(".admin-assistant-toggle");
    var panel = box.querySelector(".admin-assistant-panel");
    var log = box.querySelector(".admin-assistant-log");
    var chips = box.querySelector(".admin-assistant-chips");
    var form = box.querySelector(".admin-assistant-form");
    var input = form.querySelector("input");
    var links = Array.prototype.slice.call(box.querySelectorAll(".admin-assistant-links a"));
    var started = false;

    var guides = [
        {
            words: ["add product", "new product", "create product"],
            text: "Products, then Add product. Fill in the name, price, stock and category, upload the images and save. Untick Visible to keep it hidden from the shop."
        },
        {
            words: ["stock", "out of"],
            text: "Open a product and change its Stock. Products with low stock are listed under Low stock and on the Dashboard."
        },
        {
            words: ["order status", "ship", "deliver", "cancel"],
            text: "Open the order in Orders and pick the new status. The customer sees it in their account."
        },
        {
            words: ["coupon", "promo", "discount"],
            text: "Coupons holds promo codes with a percent or amount and an end date. Discount levels give regular customers an automatic discount."
        },
        {
            words: ["article", "journal", "post"],
            text: "Journal holds the articles, Community holds what customers post. Hide anything that does not belong from either list."
        },
        {
            words: ["terms", "privacy", "page", "footer", "phone", "address", "contact"],
            text: "Pages edits Terms and other site pages, Settings edits the phone, address and footer texts. Changes show on the site in a few minutes."
        },
        {
            words: ["role", "staff", "admin user", "permission"],
            text: "Roles is for the super admin: it decides who can enter the panel and what they see."
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

    function escape(text) {
        return String(text).replace(/&/g, "&amp;").replace(/</g, "&lt;");
    }

    function linkHtml(link) {
        return "<a href=\"" + link.getAttribute("href") + "\">" + link.getAttribute("data-title") + "</a>";
    }

    function menu() {
        chips.innerHTML = "";
        var items = [{ label: "What needs attention?" }];
        links.filter(function (link) {
            return ["Pending orders", "Add product", "Low stock"].indexOf(link.getAttribute("data-title")) >= 0;
        }).forEach(function (link) {
            items.push({ label: link.getAttribute("data-title"), link: link });
        });
        items.forEach(function (item) {
            var button = document.createElement("button");
            button.type = "button";
            button.textContent = item.label;
            button.addEventListener("click", function () {
                say(item.label, true);
                if (item.link) window.location.href = item.link.getAttribute("href");
                else summary();
            });
            chips.appendChild(button);
        });
    }

    function summary() {
        fetch(box.getAttribute("data-summary"), { credentials: "same-origin", headers: { "Accept": "application/json" } })
            .then(function (response) { return response.ok ? response.json() : null; })
            .then(function (data) {
                if (!data || !data.available) {
                    say("I could not load the numbers right now. Try again in a moment.");
                    return;
                }
                var lines = [];
                lines.push(data.pendingOrders + " pending order(s), " + data.ordersToday + " new today.");
                lines.push("Revenue this month: " + Number(data.revenueThisMonth).toFixed(2) + " AZN, " + data.newCustomers + " new customer(s).");
                if (data.lowStock.length) {
                    lines.push("Low stock: " + data.lowStock.map(function (p) { return escape(p.Name || p.name) + " (" + (p.Stock !== undefined ? p.Stock : p.stock) + ")"; }).join(", ") + ".");
                } else {
                    lines.push("Stock looks fine.");
                }
                say(lines.join("<br>"));
            })
            .catch(function () {
                say("I could not load the numbers right now. Try again in a moment.");
            });
    }

    function reply(question) {
        var text = question.toLowerCase();
        if (text.indexOf("attention") >= 0 || text.indexOf("today") >= 0 || text.indexOf("summary") >= 0 || text.indexOf("status") === 0) {
            summary();
            return;
        }
        var guide = guides.filter(function (item) {
            return item.words.some(function (word) { return text.indexOf(word) >= 0; });
        })[0];
        var found = links.filter(function (link) {
            return text.indexOf(link.getAttribute("data-title").toLowerCase()) >= 0 || link.getAttribute("data-title").toLowerCase().indexOf(text) >= 0;
        });
        var parts = [];
        if (guide) parts.push(guide.text);
        if (found.length) parts.push("Go to: " + found.slice(0, 4).map(linkHtml).join(", "));
        var fallback = parts.length ? parts.join("<br>") : "I did not find that. Try a section name like Orders or Coupons, or ask how to add a product, change stock or edit a page.";

        var wait = document.createElement("div");
        wait.textContent = "...";
        log.appendChild(wait);
        log.scrollTop = log.scrollHeight;

        var body = new URLSearchParams({ question: question, __RequestVerificationToken: box.getAttribute("data-csrf") || "" });
        fetch(box.getAttribute("data-ask"), {
            method: "POST",
            credentials: "same-origin",
            headers: { "Content-Type": "application/x-www-form-urlencoded", "X-Requested-With": "XMLHttpRequest" },
            body: body.toString()
        })
            .then(function (response) { return response.ok ? response.json() : null; })
            .then(function (data) {
                wait.remove();
                if (data && data.answer) {
                    var html = escape(data.answer).replace(/\n/g, "<br>");
                    if (found.length) html += "<br>Go to: " + found.slice(0, 4).map(linkHtml).join(", ");
                    say(html);
                } else {
                    say(fallback);
                }
            })
            .catch(function () {
                wait.remove();
                say(fallback);
            });
    }

    function open() {
        panel.hidden = false;
        box.classList.add("is-open");
        toggle.setAttribute("aria-expanded", "true");
        if (!started) {
            started = true;
            say("Hi! I can show what needs attention, take you to a section or explain how to do a task.");
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
    box.querySelector(".admin-assistant-close").addEventListener("click", close);

    document.addEventListener("keydown", function (event) {
        if (event.key === "Escape" && !panel.hidden) close();
    });

    form.addEventListener("submit", function (event) {
        event.preventDefault();
        var question = input.value.trim();
        if (!question) return;
        input.value = "";
        say(question, true);
        reply(question);
    });
})();
