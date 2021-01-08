
$.getJSON("http://localhost:4242/get-prices/", function (data) {
    var output = '<select name="prods" id="prods">';
    $.each(data.data, function (key, val) {
        //for debug
        console.log(data);
        $.getJSON("http://localhost:4242/get-prods/" + val.product, function (data) {
            //for debug        
            console.log(data);
            console.log(val.id);
            console.log(data.name);
            var line = '<option value="' + val.id + '">' + data.name + '</option>';
            console.log(line);
            output += line;

        });

    }); //get JSON

    console.log(output);
    setTimeout(function () {
        output += '</select>';
        $('#update').html(output);
    }, 1000);

    console.log(output);

});

var handleFetchResult = function (result) {
    console.log(result);
    if (!result.ok) {
        alert("There has been an error updating your subscription!");

        return "There has been an error updating your subscription!"
    }
    else {

        alert("Subscription has been updated!");
        return "Subscription has been updated!";
    }
    return result.json();
};

var handleFetchResult1 = function (result) {
    console.log(result);
    if (!result.ok) {
        alert("There has been an error cancelling your subscription!");

        return "There has been an error cancelling your subscription!"
    }
    else {

        alert("Subscription has been cancelled!");
        return "Subscription has been cancelled!";
    }
    return result.json();
};
var showErrorMessage = function (message) {
    var errorEl = document.getElementById("error-message")
    errorEl.textContent = message;
    errorEl.style.display = "block";
};

$("#updatebtn").click(function () {
    console.log("click");
    var priceid = $('#prods').val();
    console.log($('#prods').val());
    console.log($('#sidd').val());
    var id = $('#sidd').val();
    var data = JSON.stringify({
        priceid: priceid,
        id: id
    });
    console.log(data);

    return fetch("/update-subs-item", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: data
    }).then(handleFetchResult);
})
$("#deletebtn").click(function () {
    var searchField = $('#search').val();

    return fetch("/delete-subs/" + searchField, {
        method: "DELETE",
        headers: {
            "Content-Type": "application/json"
        },
    }).then(handleFetchResult1);
    
})
$('#search').keyup(function () {

    var urlForJson = "http://localhost:4242/get-subs/";
    var urlForSubItem = "http://localhost:4242/get-subs-item/";
    var urlForProd = "http://localhost:4242/get-prods/";

    var searchField = $('#search').val();
    $.getJSON(urlForJson + searchField).done(function (data) {
        console.log(data);
        console.log(data.items.data[0].id);
        $.getJSON(urlForSubItem + data.items.data[0].id, function (data) {
            //for debug
            console.log(data);
            $('#sid').html('<input id="sidd" type="text" value="' + data.id + '">' + data.id + '</input>');

            $.getJSON(urlForProd + data.price.product, function (data) {
                var output = '<p>Current Plan: ' + data.name + '</p>';
                //for debug
                console.log(data.name);
                $('#current').html(output);
            });
        });
    })
        .fail(function (jqxhr) {
            var output = '<p>Please enter a valid subscription ID!</p>';
            //for debug
            $('#current').html(output);
        });

});
