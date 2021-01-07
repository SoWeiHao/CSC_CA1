$('#search').keyup(function () {
    //get data from json file
    //var urlForJson = "data.json";
   

    //get data from Restful web Service in development environment
    var urlForJson = "https://localhost:44337/api/talents";

    //get data from Restful web Service in production environment
    //var urlForJson= "http://csc123.azurewebsites.net/api/talents";
    console.log("Retrieving data from Restful Web Service");
    //Url for the Cloud image hosting
    var urlForCloudImage = "https://res.cloudinary.com/soweihao/image/upload/v1606996671/SearchTalent/";
    var searchField = $('#search').val();

    if (searchField.match(/^([a-z\(\) ]+)$/i) || searchField=="") {
        var myExp = new RegExp(searchField, "i");
        $.getJSON(urlForJson, function (data) {
            var output = '<ul class="searchresults">';
            $.each(data, function (key, val) {
                //for debug
                console.log(data);
                if ((val.Name.search(myExp) != -1) ||
                    (val.Bio.search(myExp) != -1)) {
                    output += '<li>';
                    output += '<h2>' + val.Name + '</h2>';
                    //get the absolute path for local image
                    //output += '<img src="images/'+ val.ShortName +'_tn.jpg" alt="'+ val.Name +'" />';

                    //get the image from cloud hosting
                    output += '<img src=' + urlForCloudImage + val.ShortName + "_tn.jpg alt=" + val.Name + '" />';
                    output += '<p>' + val.Bio + '</p>';
                    output += '</li>';
                }
            });
            output += '</ul>';
            $('#update').html(output);

        }); //get JSON
    }
    else {
        $('#update').html('<p>Please enter only letters</p>');
}
});
