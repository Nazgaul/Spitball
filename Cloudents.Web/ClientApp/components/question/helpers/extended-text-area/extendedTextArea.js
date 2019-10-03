import FileUpload from 'vue-upload-component/src'; //docs here https://lian-yue.github.io/vue-upload-component

export default {
    props: {
        value: {type: String},
        error: {},
        actionType: {type: String, default: 'answer'},
        isFocused: false,
        uploadUrl: {type: String},
        openNewBaller: {
            type: Function,
            required: false
        },
        isAttachVisible: {type: Boolean, default: true, required: false}
    },
    components: {
        FileUpload
    },
    data() {
        return {
            fullPreview: false,
            errorTextArea: {},
            files: [],
            extensions: ['jpeg', 'jpe', 'jpg', 'gif', 'png', 'webp', 'bmp'],
            componentUniqueId: `instance-${this._uid}`,
            uploadFileError: false
        };
    },
    computed: {
        setPlaceholder() {
            return `extendedTextArea_type_your_${this.actionType}`
        }
    },
    methods: {
        updateValue: function (value) {
            this.$emit('input', value);
        },
        togglePreview: function () {
            this.fullPreview = !this.fullPreview;
        },
        //will remove from files[]
        remove(file) {
            this.$refs.upload.remove(file);
        },
        deletePreview: function (file, index) {
            this.remove(file); //remove from files[]
            this.$emit('removeFile', index); // remove from files list in parent newQuesiton component
        },

        inputFile: function (newFile, oldFile) {
            let self = this;
            if (self.files && self.files.length > 4) {
                return;
            }
            if (newFile && newFile.progress) {
                // console.log('progress', newFile.progress, newFile)
            }
            // Upload error
            if (newFile && oldFile && newFile.error !== oldFile.error) {
                // console.log('error', newFile.error, newFile)
            }
            // Get response data
            if (newFile && oldFile && !newFile.active && oldFile.active) {
                // Get response data
                // console.log('response', newFile.response);
                if (newFile.xhr) {
                    //  Get the response status code
                    // console.log('status', newFile.xhr.status)
                    if (newFile.xhr.status === 200) {
                        // console.log('Succesfully uploadede')
                        //on after successful loading done, emit to parent to add to list
                        if (newFile.response && newFile.response.files) {
                            self.$emit('addFile', newFile.response.files);
                        }

                    } else {
                        // console.log('error, not uploaded')
                    }
                }
                if (newFile && newFile.response && newFile.response.status === 'success') {
                    //generated blob name from server after successful upload
                }
            }
            if (Boolean(newFile) !== Boolean(oldFile) || oldFile.error !== newFile.error) {
                if (!this.$refs.upload.active) {
                    this.$refs.upload.active = true;
                }
            }
        },

        inputFilter: function (newFile, oldFile, prevent) {
            if (newFile && !oldFile) {
                //prevent adding new files if maximum reached
                if (this.files.length >= 4) {
                    return prevent();
                }
                // Filter non-supported extensions  both lower and upper case
                let patt1 = /\.([0-9a-z]+)(?:[\?#]|$)/i;
                let ext = (`${newFile.name}`.toLowerCase()).match(patt1)[1];
                let isSupported = this.extensions.includes(ext);
                if (!isSupported) {
                    this.uploadFileError = true;
                    setTimeout(() => {
                        this.uploadFileError = false;
                    }, 3000);
                    return prevent();
                }
                if (newFile.size === 0) {
                    return prevent();
                }
            }
            if (newFile && (!oldFile || newFile.file !== oldFile.file)) {
                // Create a blob field
                newFile.blob = '';
                let URL = window.URL || window.webkitURL;
                if (URL && URL.createObjectURL) {
                    newFile.blob = URL.createObjectURL(newFile.file);
                }
            }
        },
        isNewBaller() {
            this.openNewBaller();
        }

    },
    mounted() {
        this.$root.$on('previewClean', () => {
            return this.files = [];
        });

    },
    created() {
        console.log(`Oneeee!! !!!router path: ${this.$route.fullPath} component`, this);
    }
}