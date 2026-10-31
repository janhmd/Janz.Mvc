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
