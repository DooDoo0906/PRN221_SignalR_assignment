// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

//const { signalR } = require("../microsoft/signalr/dist/browser/signalr");

// Write your JavaScript code.
$(() => {
    LoadAppUserData();
    LoadPostCateData();
    LoadPostData();
    var connection = new signalR.HubConnectionBuilder().withUrl("/signalrServer").build();
    connection.start();

    connection.on("LoadAppUsers", function () {
        LoadAppUserData();
    })
    connection.on("LoadPostCategories", function () {
        LoadPostCateData();
    })
    connection.on("LoadPosts", function () {
        LoadPostData();
    })

    LoadAppUserData();
    LoadPostCateData();
    LoadPostData();

    function LoadAppUserData() {
        var tr = '';
        $.ajax({
            url: '/AppUsers/GetAppUsers',
            method: 'GET',
            success: (result) => {
                $.each(result, (k, v) => {
                    tr += `<tr>
                        <td> ${v.fullName} </td>
                        <td> ${v.address} </td>
                        <td> ${v.password} </td>
                        <td> ${v.email} </td>
                        <td>
                <a href='../AppUsers/Edit/${v.userId}'>Edit</a> |
                <a href='../AppUsers/Details/${v.userId}'>Details</a> |
                <a href='../AppUsers/Delete/${v.userId}'>Delete</a>
                </td> </tr>`
                })
                $("#tableBody").html(tr);
                /*$('#tableBody').pagination({
                    dataSource: tr,
                    pageSize: 5,
                    autoHidePrevious: true,
                    autoHideNext: true,
                    callback: function (data, pagination) {
                        // template method of yourself
                        var html = template(data);
                        dataContainer.html(html);
                    }
                });*/
            },
            error: (error) => {
                console.log(error)
            }
        });
    }
    function LoadPostCateData() {
        var tr = '';
        $.ajax({
            url: '/PostCategories/GetPostCates',
            method: 'GET',
            success: (result) => {
                $.each(result, (k, v) => {
                    tr += `<tr>
                        <td> ${v.categoryName} </td>
                        <td> ${v.description} </td>
                        <td>
                <a href='../PostCategories/Edit/${v.categoryId}'>Edit</a> |
                <a href='../PostCategories/Details/${v.categoryId}'>Details</a> |
                <a href='../PostCategories/Delete/${v.categoryId}'>Delete</a>
                </td> </tr>`
                })
                $("#tableBody1").html(tr);
            },
            error: (error) => {
                console.log(error)
            }
        });
    }
    function LoadPostData() {
        var tr = '';
        $.ajax({
            url: '/Posts/GetPosts',
            method: 'GET',
            success: (result) => {
                $.each(result, (k, v) => {
                    tr += `<tr>
                        <td> ${v.createdDate} </td>
                        <td> ${v.updatedDate} </td>
                        <td> ${v.title} </td>
                        <td> ${v.content} </td>
                        <td> ${v.publishStatus} </td>
                        <td> ${v.authorId} </td>
                        <td> ${v.categoryId} </td>
                        <td>
                <a href='../Posts/Edit/${v.postId}'>Edit</a> |
                <a href='../Posts/Details/${v.postId}'>Details</a> |
                <a href='../Posts/Delete/${v.postId}'>Delete</a>
                </td> </tr>`
                })
                $("#tableBody2").html(tr);
            },
            error: (error) => {
                console.log(error)
            }
        });
    }

})
