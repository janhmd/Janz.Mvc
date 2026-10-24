// Admin panel: gündüz/gecə rejimi və silmədən əvvəl soruşmaq
// Gündüz/gecə: saytla eyni açar (janz-theme), ona görə seçim hər iki tərəfdə eyni qalır
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

// Bildiriş mesajı bir neçə saniyədən sonra itir
document.querySelectorAll(".admin-flash").forEach(function (flash) {
    setTimeout(function () { flash.classList.add("is-hidden"); }, 5000);
});

// Məhsul forması: seçilən şəkillər göndərmədən əvvəl göstərilir
document.querySelectorAll("[data-preview-input]").forEach(function (input) {
    var box = document.querySelector('[data-preview="' + input.getAttribute("data-preview-input") + '"]');
    if (!box) return;
    var single = !input.multiple;
    var original = box.innerHTML; // redaktədə köhnə əsas şəkil: fayl ləğv olunsa geri qayıdır

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
            // 2 MB-dan böyük fayl elə burada qeyd olunur (server də yoxlayır)
            label.textContent = file.size > 2 * 1024 * 1024 ? "Larger than 2 MB" : (single ? "New main" : "New");
            if (file.size > 2 * 1024 * 1024) figure.classList.add("is-invalid");
            caption.appendChild(label);
            figure.appendChild(image);
            figure.appendChild(caption);
            box.appendChild(figure);
        });
    });
});

// Qalereyadan silmək: yadda saxlayana qədər "Undo" ilə geri qaytarmaq olur
document.querySelectorAll("[data-remove-toggle]").forEach(function (box) {
    box.addEventListener("change", function () {
        var figure = box.closest(".admin-thumb");
        figure.classList.toggle("is-removed", box.checked);
        box.parentElement.querySelector("span").textContent = box.checked ? "Undo" : "Remove";
    });
});
