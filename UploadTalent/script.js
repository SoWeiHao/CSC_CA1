$("form").on("submit", function (event) {
    document.getElementById("imgdiv").innerHTML = "";

    event.preventDefault();
    //Url for the Cloud image hosting
    var bucketName = "cscasgn-task-5";
    var bucketRegion = "ap-southeast-1";
    var IdentityPoolId = "";

    AWS.config.update({
        region: bucketRegion,
        credentials: new AWS.CognitoIdentityCredentials({
            IdentityPoolId: IdentityPoolId
        })
    });

    var s3 = new AWS.S3({
        apiVersion: '2006-03-01',
        params: { Bucket: bucketName }
    });
    var files = document.getElementById('img').files;
    var file = files[0];
    if (file != null) {
        var filePath = file.name;
        var fileUrl = 'https://' + bucketName + '.s3-ap-southeast-1.amazonaws.com/' + filePath;
        var fileprogress = document.getElementById('fileprogress');

        s3.upload({
            Key: filePath,
            Body: file,
            ACL: 'public-read'
        }, function (err, data) {
            console.log(data);
            if (err) {
                console.log(err);
                alert("Cannot connect to AWS server, please try again later.");
            }
            getShortenedURL(fileUrl);
        }).on('httpUploadProgress', function (progress) {
            let progressPercentage = Math.round(progress.loaded / progress.total * 100);
            console.log(progressPercentage);
            fileprogress.value = progressPercentage;


        });


        setTimeout(function () {

        }, 5000);
        console.log(fileUrl);
    }
    else {
        alert("File is null!");
    }
})

function getShortenedURL(fileUrl) {
    var form = {
        "long_url": fileUrl
    };
    const accessToken = "";
    $.ajax({
        url: "https://api-ssl.bitly.com/v4/shorten",
        cache: false,
        dataType: "json",
        type: "POST",
        method: "POST",
        contentType: "application/json",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Authorization", "Bearer " + accessToken);
        },
        data: JSON.stringify(form),
        success: function (results) {
            $("#url").append(results.link);
            $("#url").attr("href", results.link);
            console.log(JSON.stringify(results.link));
            var img = document.createElement("img");
            img.src = fileUrl;
            var src = document.getElementById("imgdiv");
            src.appendChild(img);
            alert('Successfully Uploaded! Shortened URL: ' + JSON.stringify(results.link).replaceAll("\"", ""));

        },
        error: function (results) {
            console.log(JSON.stringify(results));
        }
    });
};
