var Uploader = require('html5-uploader');

export default {
    props: {text: {type: String}},
    data() {
        return {
            files: [],
            previewList:[]
        }
    },
    methods: {
        // uploadFiles(){
        //     var files = this.$el.querySelector("#file-input").files;
        //
        //     debugger;
        // }

    },
    mounted() {
        //TODO: continue with html5-uploader, examples here: http://mpangrazzi.github.io/
        debugger;
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
                }
            }
        });
        multiple.on('file:preview', function(file, $img) {

            if ($img) {

                $img.style.maxWidth = '100%';
                $img.style.maxHeight = '100%';

                $preview.appendChild($img);
                self.previewList.push($img);
            }

        });
    }

}