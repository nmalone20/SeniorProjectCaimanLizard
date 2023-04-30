﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// navbar behavior
$(window).scroll(function () {
    $("nav").toggleClass("shadow", $(this).scrollTop() > 0);
});

// theme selection
$(document).ready(function () {
    $("#themeList a").on("click", function (e) {
        e.preventDefault();
        console.log("ready to change theme");
        var chosenTheme = $(this).text().toLowerCase();
        console.log("changed to "+ chosenTheme);
        $.ajax({
            type: "POST",
            url: "/User/SwitchTheme",
            data: { theme: chosenTheme },
            dataType: "json",
            success: function (response) {
                var theme = response.theme;
                $("#selectedTheme").text(theme);
                $("html").attr("data-bs-theme", theme);
                console.log("Theme changed to " + theme);
            },
            error: function (xhr, textStatus, errorThrown) {
                console.log(xhr.status + ': ' + errorThrown);
            }
        });

        setTimeout(function() {
            console.log("manually changing theme");
            $("html").attr("data-bs-theme", chosenTheme);
        }, 3000);
    });
});
