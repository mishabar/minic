function updateCart(cart) {
    var $cart = $("#_cart");
    if (cart == null || cart.Items.length == 0) {
        $cart.find("span.cart-items").html("You cart it empty");
        $cart.find("span.cart-price").html("");
        $("#itemsQuantity").html("0 items");
        $("#itemsTotal").html("(R$ 0,00)");
    } else {
        var total = 0;
        var count = 0;
        var $items = $cart.find("ul.dropdown-menu");
        $items.html("");
        $.each(cart.Items, function (idx, item) {
            count += item.Quantity;
            total += (item.Quantity * item.Price);
            $items.append("<li><div class='cart-item'><div><img src='" + item.ImageUrl + "' /><div class='item-name'>" + item.Name + "</div></div><div><div class='item-quantity'>" + item.Quantity + " items</div><div class='item-price'>R$ " + (item.Quantity * item.Price).toFixed(2).toLocaleString() + "</div></div></div></li>");
        });
        $cart.find("span.cart-items").html(count + " items");
        $cart.find("span.cart-price").html("(R$ " + total.toFixed(2).toLocaleString() + ")");
        $("#itemsQuantity").html(count + " items");
        $("#itemsTotal").html("(R$ " + total.toFixed(2).toLocaleString() + ")");
        $items.append("<li class='checkout'><button class='btn btn-block btn-warning' onclick='goCheckout()'><i class='glyphicon glyphicon-shopping-cart'></i>&nbsp;Checkout</button></li>");
    }
}

function loadCart() {
    $.get("/Orders/MyCart", function (response) {
        if (response.error) {
            alert(response.error);
        } else {
            updateCart(response.cart);
        }
    });
}

function updateItemQuantity(sku, size, quantity) {
    $.post("/Orders/SetItemQuantity", { SKU: sku, Size: size, Quantity: quantity }, function (response) {
        updateCart(response.cart);
    });
}

function removeItem(sku, size, success) {
    $.post("/Orders/RemoveItem", { SKU: sku, Size: size }, function (response) {
        if (success) { success(); }
        updateCart(response.cart);
    });
}

function deleteItem(sku, size) {
    loadCart();
}

function goCheckout() {
    document.location.href = "/Orders/Checkout";
}