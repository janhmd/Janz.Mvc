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
