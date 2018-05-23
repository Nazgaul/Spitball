import questionService from "../../../../services/questionService";

var Uploader = require('html5-uploader');

export default {
    props: {
        value: {type: String},
        collapsed:{type:Boolean, default:false}
    },
    data() {
        return {
            files: [],
            filesFromServer: [],
            previewList: []
        }
    },
    methods: {
        updateValue: function (value) {
            this.$emit('input', value);
        },
        expandTextArea(){
            this.collapsed = false;
            // this.$emit('expand-textarea');
        }
    },
    mounted() {
        //TODO: continue with html5-uploader, examples here: http://mpangrazzi.github.io/
        var self = this;
        var multiple = new Uploader({
            el: '#file-input',
        });
        var $preview = this.$el.querySelector('#input-multiple-preview');



        multiple.on('files:added', function (uploadedFiles) {
            for (var file of uploadedFiles) {

                // TODO: check for duplications fast-deep-equal
                if (self.files.indexOf(file) === -1) {
                    self.files.push(file);
                    var formData = new FormData();
                    formData.append(file.name || 'filename', file);
                    debugger;
                    questionService.uploadFile(formData)
                        .then(function (response) {
                            debugger;
                            self.filesFromServer.push(response.fileName);
                        });
                }
            }





            // function handleFileSelect(evt) {
                var files = uploadedFiles; // FileList object

                // Loop through the FileList and render image files as thumbnails.
                for (var i = 0, f; f = files[i]; i++) {

                    // Only process image files.
                    if (!f.type.match('image.*')) {
                        continue;
                    }

                    var reader = new FileReader();

                    // Closure to capture the file information.
                    reader.onload = (function(theFile) {
                        return function(e) {
                            // Render thumbnail.
                            var span = document.createElement('span');
                            span.innerHTML = ['<img class="thumb" src="', e.target.result,
                                '" title="', escape(theFile.name), '"/>'].join('');
                            document.getElementById('list').insertBefore(span, null);
                        };
                    })(f);

                    // Read in the image file as a data URL.
                    reader.readAsDataURL(f);
                }
            // }


        });
        multiple.on('file:preview', function (file, $img) {
            if ($img) {
                self.previewList.push($img.outerHTML);
            }
        });
    }

}