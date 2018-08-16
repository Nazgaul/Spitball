import colorsSet from '../colorsSet';
var Uploader = require('html5-uploader');
// import VueQuillEditor from 'vue-quill-editor'

export default {

    props: {
        value: {type: String},
        error: {},
        actionType:{type:String, default:'answer'},
        isFocused: false,
        uploadUrl: {type: String}
    },
    data() {
        return {
            previewList: [],
            fullPreview:false,
            errorTextArea :{},
            colorsSet: colorsSet,
            activeColor: 0,
            counter: 0,
            uploadLimit: 4
            }
    },
    watch:{
        value(newVal,oldVal){
            //clean preview list when text is empty
            //if(!newVal){this.previewList=[];}
        }
    },
    methods: {
        updateValue: function (value) {
            this.$emit('input', value);
        },
        togglePreview: function(){this.fullPreview = !this.fullPreview},
        deletePreview: function(index){
            this.counter = this.counter -1;
            this.previewList.splice(index,1);
            this.$emit('removeFile', index);
        },
        updateColor(color){
           this.activeColor = color || colorsSet[0];
           this.$parent.$emit('colorSelected', this.activeColor);
        }
    },
    mounted() {
        //Used html5-uploader, examples here: http://mpangrazzi.github.io/
        var self = this;
        var multiple = new Uploader({
            el: '#file-input',
            url: this.uploadUrl
        });

        multiple.on('files:added', function (val) {
            self.errorTextArea.errorText = '4 files only';
            if(val.length <= self.uploadLimit){
            this.files=val.filter(i=>i.type.indexOf("image")>-1);
             this.upload()
            }
        });

        multiple.on('file:preview', function (file, $img) {
             let files = this.getFiles();
            if ($img && files.length < self.uploadLimit) {
                self.counter = self.counter + 1;
                // self.previewList.push($img.outerHTML);
                if(self.counter <= self.uploadLimit){
                    self.previewList.push($img.src);
                }

            }
        });

        multiple.on('upload:done', function (response) {
            self.$emit('addFile', JSON.parse(response).files.toString());
        });
        multiple.on('upload:progress', function (progress) {
            console.log('progress: %s', progress);
        });

    },

}