import colorsSet from '../colorsSet';
// var Uploader = require('html5-uploader');
import Vue from 'vue';
import FileUpload from 'vue-upload-component/src';

Vue.component('file-upload', FileUpload);
export default {

    props: {
        value: {type: String},
        error: {},
        actionType: {type: String, default: 'answer'},
        isFocused: false,
        uploadUrl: {type: String}
    },
    data() {
        return {
            previewList: [],
            fullPreview: false,
            errorTextArea: {},
            colorsSet: colorsSet,
            activeColor: {
                name: 'default',
                id: 0
            },
            counter: 0,
            uploadLimit: 4,
            isFirefox: global.isFirefox,
            files: [],
            uploadedFiles: []
        }


    },
    watch: {
        files(newVal, oldVal) {
         console.log('files',this.files)
        }
    },
    methods: {
        updateValue: function (value) {
            this.$emit('input', value);
        },
        togglePreview: function () {
            this.fullPreview = !this.fullPreview
        },
        deletePreview: function (index) {
            this.counter = this.counter - 1;
            this.previewList.splice(index, 1);
            this.$emit('removeFile', index);
        },
        updateColor(color) {
            this.activeColor = color || colorsSet[0];
            this.$parent.$emit('colorSelected', this.activeColor);
        },

        inputFile: function (newFile, oldFile) {
            let self= this;
            if (newFile && newFile.progress) {
                console.log('progress', newFile.progress, newFile)
            }
            // Upload error
            if (newFile && oldFile && newFile.error !== oldFile.error) {

                console.log('error', newFile.error, newFile)
            }
            // Get response data
            if (newFile && oldFile && !newFile.active && oldFile.active) {
                // Get response data
                console.log('response', newFile.response);
                if (newFile.xhr) {
                    //  Get the response status code
                    console.log('status', newFile.xhr.status)
                    if (newFile.xhr.status === 200) {
                        console.log('Succesfully uploadede')
                        if(newFile.response && newFile.response.files){
                            this.uploadedFiles = this.uploadedFiles.concat(newFile.response.files);
                            if(this.uploadedFiles.length  <= 4){
                                self.$emit('addFile', newFile.response.files);
                            }

                        }

                    } else {
                        console.log('error, not uploaded')
                    }
                }
                if (newFile && newFile.response && newFile.response.status === 'success') {
                    //generated blob name from server after successful upload
                }
            }
            if (Boolean(newFile) !== Boolean(oldFile) || oldFile.error !== newFile.error) {
                if (!this.$refs.upload.active) {
                    this.$refs.upload.active = true
                }
            }
        },

        inputFilter: function (newFile, oldFile, prevent) {
            if (newFile && !oldFile) {
                // Filter non-image file
                if (!/\.(jpeg|jpe|jpg|gif|png|webp)$/i.test(newFile.name)) {
                    return prevent()
                }
            }
            if (newFile && (!oldFile || newFile.file !== oldFile.file)) {
                // Create a blob field
                // 创建 blob 字段
                newFile.blob = ''
                let URL = window.URL || window.webkitURL;
                if (URL && URL.createObjectURL) {
                    newFile.blob = URL.createObjectURL(newFile.file);
                    if(this.previewList.length >= 4)return;
                    this.previewList.push(newFile.blob)
                }
                // Thumbnails
                // 缩略图
                newFile.thumb = ''
                if (newFile.blob && newFile.type.substr(0, 6) === 'image/') {
                    newFile.thumb = newFile.blob
                }
            }



}

    },
    mounted() {
        //Used html5-uploader, examples here: http://mpangrazzi.github.io/
        // var self = this;
        // var multiple = new Uploader({
        //     el: '#file-input',
        //     url: this.uploadUrl
        // });
        //
        // multiple.on('files:added', function (val) {
        //     self.errorTextArea.errorText = '4 files only';
        //     if(val.length <= self.uploadLimit){
        //     this.files=val.filter(i=>i.type.indexOf("image")>-1);
        //      this.upload()
        //     }
        // });
        //
        // multiple.on('file:preview', function (file, $img) {
        //     let files = this.getFiles();
        //     if ($img && files.length < self.uploadLimit) {
        //         self.counter = self.counter + 1;
        //         // self.previewList.push($img.outerHTML);
        //         if(self.counter <= self.uploadLimit){
        //             self.previewList.push($img.src);
        //         }
        //
        //     }
        // });

        // multiple.on('upload:done', function (response) {
        //     self.$emit('addFile', JSON.parse(response).files.toString());
        // });
        // multiple.on('upload:progress', function (progress) {
        //     console.log('progress: %s', progress);
        // });

        this.$root.$on('colorReset', () => {
           return self.activeColor = {
                name: 'default',
                id: 0
            }
        });
        this.$root.$on('previewClean', () => {
           return this.previewList = [];
        });

    },

}