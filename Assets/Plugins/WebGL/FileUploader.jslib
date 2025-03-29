var FileUploader = {
    UploadImage: function (gameObjectName, methodName) {
        let input = document.createElement('input');
        input.type = 'file';
        input.accept = 'image/*';
        document.body.appendChild(input);

        input.onchange = function (event) {
            let file = event.target.files[0];
            let reader = new FileReader();

            reader.onload = function (e) {
                let base64String = e.target.result.split(',')[1];
                SendMessage(gameObjectName, methodName, base64String);
            };

            reader.readAsDataURL(file);
        };

        input.click();
    }
};

mergeInto(LibraryManager.library, FileUploader);
