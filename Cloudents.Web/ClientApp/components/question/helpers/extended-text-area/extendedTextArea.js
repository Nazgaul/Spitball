//import questionService from "../../../../services/questionService";

var Uploader = require('html5-uploader');

export default {
    props: {
        value: {type: String},
        error: {},
        actionType:{type:String,default:'answer'},
        isFocused: false,
        uploadUrl: {type: String}
    },
    data() {
        return {
            previewList: [],
            fullPreview:false,
            errorTextArea :{}
            }
    },
    watch:{
        value(newVal,oldVal){
            //clean preview list when text is empty
            if(!newVal){this.previewList=[];}
        }
    },
    methods: {
        updateValue: function (value) {
            this.$emit('input', value);

        },
        togglePreview: function(){this.fullPreview = !this.fullPreview},
        deletePreview: function(index){
           // debugger;
            this.previewList.splice(index,1);
            this.$emit('removeFile', index);
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
            this.files=val.filter(i=>i.type.indexOf("image")>-1);
            if(this.files.length){
               // debugger;
                this.upload()
            }
        });

        multiple.on('file:preview', function (file, $img) {
            if ($img) {
                // self.previewList.push($img.outerHTML);
                self.previewList.push($img.src);
            }
        });

        multiple.on('upload:done', function (response) {
          //  debugger;
            self.$emit('addFile', JSON.parse(response).files.toString());
        });
        multiple.on('upload:progress', function (progress) {
            console.log('progress: %s', progress);
        });

    }

}