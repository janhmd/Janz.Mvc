// İstifadəçi profili: üstünlüklər tabları, dairəvi diaqram, izləmə və paylaşma
// Hələlik nümunə məlumatlarla; MVC-də profil və statistika API-dən gələcək
(function () {
    var page = document.getElementById("user-profile");
    if (!page) return;

    // Rənglər CSS-dədir (--up-c0 … --up-c8), gecə və gündüz temasında ayrı
    var colorCount = 9;
    var svgNs = "http://www.w3.org/2000/svg";

    // Siyahıdakı faizlərdən halqa diaqramı çəkir (hər hissə arasında kiçik boşluq)
    page.querySelectorAll(".up-pref").forEach(function (pref) {
        var items = pref.querySelectorAll("li");
        var svg = pref.querySelector(".up-donut");
        var total = 0;
        items.forEach(function (item) { total += parseFloat(item.getAttribute("data-value")) || 0; });
        var radius = 15.915;
        var offset = 25;

        // Diaqramın üzərinə gələndə hissənin adı və faizi görünür
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

    // İzləmə: düymə və izləyici sayı birlikdə dəyişir; postlardakı "Follow" da eyni olur
    var follow = page.querySelector(".up-follow");
    var followers = page.querySelector("[data-followers]");
    var postFollows = page.querySelectorAll(".feed-follow");

    function setFollow(on) {
        follow.classList.toggle("is-on", on);
        follow.setAttribute("aria-pressed", on ? "true" : "false");
        follow.textContent = on ? "Following" : "Follow";
        postFollows.forEach(function (button) {
            button.classList.toggle("is-on", on);
            button.setAttribute("aria-pressed", on ? "true" : "false");
            button.textContent = on ? "Following" : "Follow";
        });
    }

    follow.addEventListener("click", function () {
        var on = !follow.classList.contains("is-on");
        followers.textContent = String((parseInt(followers.textContent, 10) || 0) + (on ? 1 : -1));
        setFollow(on);
    });

    postFollows.forEach(function (button) {
        button.addEventListener("click", function () {
            // site.js düyməni artıq dəyişib; profil düyməsini ona uyğunlaşdırırıq
            var on = button.classList.contains("is-on");
            if (on !== follow.classList.contains("is-on")) {
                followers.textContent = String((parseInt(followers.textContent, 10) || 0) + (on ? 1 : -1));
                setFollow(on);
            }
        });
    });

    page.querySelector(".up-share").addEventListener("click", function () {
        if (typeof openShare === "function") openShare(window.location.href);
    });
})();
