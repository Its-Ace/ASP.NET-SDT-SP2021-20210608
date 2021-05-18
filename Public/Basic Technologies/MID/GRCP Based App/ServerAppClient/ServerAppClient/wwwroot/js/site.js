// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function LoadProducts() {
    $.ajax({
        url: "/api/items",
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            var output = '';
            $.each(result, function (key, item) {
                output += '<tr>';
                output += '<td>' + item.itemId + '</td>';
                output += '<td>' + item.name + '</td>';
                output += '<td>' + item.seats + '</td>';
                output += '<td>' + item.brand + '</td>';
                output += '<td>' + item.type + '</td>';
                output += '<td>' + item.value + '</td>';
                output += `<td><a href="#" class="btn redgate" style = "color: black ;background-color: lightgreen;" onclick="SetUpEditModal(${item.itemId})">Edit</a> |
                        <a href="#" class="btn redgate" onclick="DeleteProduct(${item.itemId})">Delete</a></td>`;
                output += '</tr>';
            });
            $('.tbody').html(output);
        },
        error: function (message) {
            console.log(message.responseText);
        }
    });
}
function AddProduct() {
    var res = validateForm();
    if (res == false) {
        return false;
    }
    var productObj = {
        name: $('#Name').val(),
        seats: parseInt($('#Seats').val()),
        brand: $('#Brand').val(),
        type: $('#Type').val(),
        value: parseFloat($('#Value').val())
    };
    $.ajax({
        url: "/api/items/",
        data: JSON.stringify(productObj),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        success: function () {
            LoadProducts();
            $('#productModal').modal('hide');
        },
        error: function (message) {
            console.log(message.responseText);
        }
    });
}
function SetUpEditModal(id) {
    $('form input').css('border-color', 'grey');
    $('#productModal h4').text('Edit Product');
    $.ajax({
        url: "/api/items/" + id,
        typr: "GET",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (result) {
            $('#ProductID').val(result.itemId);
            $('#Name').val(result.name);
            $('#Seats').val(result.seats);
            $('#Brand').val(result.brand);
            $('#Type').val(result.type);
            $('#Value').val(result.value);
            $('#productModal').modal('show');
            $('#btnUpdateProduct').show();
            $('#btnAddProduct').hide();
        },
        error: function (message) {
            console.log(message.responseText);
        }
    });
    return false;
}
function UpdateProduct() {
    if (!validateForm()) {
        return false;
    }
    var productObj = {
        ProductID: parseInt($('#ProductID').val()),
        Name: $('#Name').val(),
        Seats: parseInt($('#Seats').val()),
        Brand: $('#Brand').val(),
        Type: $('#Type').val(),
        Value: parseFloat($('#Value').val()),
    };
    $.ajax({
        url: "/api/items/",
        data: JSON.stringify(productObj),
        type: "PUT",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function () {
            LoadProducts();
            $('#productModal').modal('hide');
            clearStuff();
        },
        error: function (message) {
            console.log(message.responseText);
        }
    });
}
function DeleteProduct(id) {
    if (confirm("Are you sure?")) {
        $.ajax({
            url: "/api/items/" + id,
            type: "DELETE",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function () {
                LoadProducts();
            },
            error: function (message) {
                console.log(message.responseText);
            }
        });
    }
}
/** Utility functions **/
function clearStuff() {
    $('form').trigger("reset");
    $('#btnUpdateProduct').hide();
    $('#productModal h4').text('Add Product');
    $('#btnAddProduct').show();
}
function validateForm() {
    var isValid = true;
    if ($('#Name').val().trim() == "") {
        $('#Name').css('border-color', '#c00');
        isValid = false;
    }
    else {
        $('#Name').css('border-color', 'grey');
    }
    if ($('#Seats').val().trim() == "") {
        $('#Seats').css('border-color', '#c00');
        isValid = false;
    }
    else {
        $('#Seats').css('border-color', 'grey');
    }
    if ($('#Brand').val().trim() == "") {
        $('#Brand').css('border-color', '#c00');
        isValid = false;
    }
    else {
        $('#Brand').css('border-color', 'grey');
    }
    if ($('#Type').val().trim() == "") {
        $('#Type').css('border-color', '#c00');
        isValid = false;
    }
    else {
        $('#Type').css('border-color', 'grey');
    }
    if ($('#Value').val().trim() == "") {
        $('#Value').css('border-color', '#c00');
        isValid = false;
    }
    else {
        $('#Value').css('border-color', 'grey');
    }
    return isValid;
}
LoadProducts();