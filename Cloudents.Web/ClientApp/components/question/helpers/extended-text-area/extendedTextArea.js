import questionService from "../../../../services/questionService";

var Uploader = require('html5-uploader');

export default {
    props: {
        value: {type: String},
        isFocused: false,
        uploadUrl: {type: String}
    },
    data() {
        return {
            previewList: [],
            fullPreview:false
        }
    },
    methods: {
        updateValue: function (value) {
            this.$emit('input', value);
        },
        togglePreview: function(){this.fullPreview = !this.fullPreview},
        deletePreview: function(index){
            this.previewList.splice(index,1);
        }
    },
    mounted() {
        //TODO: continue with html5-uploader, examples here: http://mpangrazzi.github.io/
        var self = this;
        var multiple = new Uploader({
            el: '#file-input',
            url: this.uploadUrl
        });

        multiple.on('files:added', function () {
            // this.upload()
        });

        multiple.on('file:preview', function (file, $img) {
            if ($img) {
                // self.previewList.push($img.outerHTML);
                self.previewList.push($img.src);
            }
        });

        multiple.on('upload:progress', function (progress) {
            console.log('progress: %s', progress);
        });

        multiple.on('upload:done', function (response) {
            self.$emit('addFile', JSON.parse(response).fileName);
        });
    }

}