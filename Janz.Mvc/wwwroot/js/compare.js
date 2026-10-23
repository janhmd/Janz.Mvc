// Müqayisə səhifəsi: seçilən məhsullar yan-yana, "+" ilə pəncərədən seçilir.
// Məhsullar səhifəyə serverdən gəlir (#compare-products), seçim brauzerdə saxlanılır (site.js: readCompare / writeCompare)
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
        // Silinmiş məhsullar (API-də artıq yoxdur) siyahıdan avtomatik çıxır
        var chosen = readCompare().map(findProduct).filter(Boolean);
        table.innerHTML = "";
        table.style.setProperty("--compare-cols", chosen.length + 1);
        table.classList.toggle("is-empty", !chosen.length);
        diffBox.disabled = chosen.length < 2;

        var head = el("div", "compare-row compare-head");
        head.appendChild(el("div", "compare-label"));
        chosen.forEach(function (item) {
            var card = el("article", "compare-card");
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

            // Səbətə atmaq site.js-dəki ümumi [data-cart-add] düyməsi ilə olur (header paneli də yenilənir)
            var buy = el("button", "compare-buy");
            buy.type = "button";
            buy.setAttribute("data-cart-add", item.id);
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

    // "Yalnız fərqlər": hamısında eyni olan sətirlər gizlənir
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
        // Əvvəl seçilənlər öz sırasında qalır, yenilər sona əlavə olunur
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
