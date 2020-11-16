﻿$(function () {
    //Bắt sự kiện uploadfile
    $('body').on('change', 'input[idata=FileUploadUserController]', function () {
        var cmd = $(this);
        var boxRender = cmd.parent().parent().attr('idata');

        var result = FileUploadController.PreviewImage(cmd);
        result.success(function (data) {
            $("#" + boxRender).find("#Box_" + boxRender).find("#ImageAvatar").attr("src", data);
        });
    });

    //Bắt sự kiện ko xóa ảnh đi
    $('body').on('click', '.btnRemoveImage', function () {
        var cmd = $(this);
        var boxRender = cmd.parent().parent().parent().parent();
        var customerid = boxRender.find("#hidCustomerId").val();

        if (customerid === "") {
            boxRender.find('input[idata=FileUploadUserController]').val('');
            boxRender.find("#ImageAvatar").attr('src', '/Content/Image/default.jpg');
        } else {
            var t = FileUploadController.RemoveImage(customerid);
            t.success(function (data) {
                if (data.isSuccess) {
                    boxRender.find("#ImageAvatar").attr('src', '/Content/Image/default.jpg');
                } else {
                    toastr.success(data.Message);
                }
            });
        }
    });
});

var FileUploadController = {
    boxRender: "",
    nameFileUpload: "",
    filePathUrl: "",
    customerId: "",
    init: function (boxRender, nameFileUpload, filePathUrl, customerId) {
        this.boxRender = boxRender;
        this.nameFileUpload = nameFileUpload;
        this.filePathUrl = filePathUrl;
        this.customerId = customerId;

        var a = new FileUploadClass(FileUploadController.boxRender, FileUploadController.nameFileUpload, FileUploadController.filePathUrl, FileUploadController.customerId);

        a.LoadData();
    },
    PreviewImage: function (inputupload) {
        var file = inputupload.prop("files");
        
        var formdata = new FormData();

        for (i = 0; i < file.lenght; i++) {
            //Appending each file to FormData object
            formdata.append(file[i].name, file[i]);
        }

        return $.ajax({
            url: _prefixParkingDomain + '/tblCard/PreviewImageUpload',
            data: formdata,
            type: "POST",
            contentType: false,
            processData: false
        });
    },
    RemoveImage: function (customerid) {
        return FunctionHelperController.Delete(customerid, '/CommonUse/RemoveImage');
    }
};

class FileUploadClass {
    constructor(boxRender, nameFileUpload, filePathUrl, customerId) {
        this.boxRender = boxRender;
        this.nameFileUpload = nameFileUpload;
        this.filePathUrl = filePathUrl;
        this.customerId = customerId;
    }

    LoadData() {
        var cmd = $("#" + this.boxRender);

        var model = {
            FileUploadName: this.nameFileUpload,
            BoxRenderId: this.boxRender,
            FilePath: this.filePathUrl,
            Base64String: "",
            CustomerId: this.customerId
        };
        var result = FunctionHelperController.LoadData(model, '/$/PartialImagePreview');
                result.success(function (data) {
                    cmd.html(data);



            //JSFileUpload();
        });
    }
}