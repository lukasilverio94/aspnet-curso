$(document).ready(function () {
    console.log("Site Javascript loaded successfully!");

    $('button').click(function () {
        $(this).addClass('clicked');
    })
})