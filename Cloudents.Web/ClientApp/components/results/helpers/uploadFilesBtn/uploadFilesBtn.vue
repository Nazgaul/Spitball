<template>
    <v-flex :class="['result-cell', 'mb-3', 'upload-files-action-card', 'xs-12', isFloatingBtn ? 'floatingcard' : 'regularCard']">
        <a class="mb-5 upload-link" @click="openUploaderDialog()">
            <div :class="['upload-wrap', isFloatingBtn ? 'floating-upload' : '']">
                <div class="static-center">
                    <p v-show="$vuetify.breakpoint.smAndUp"  :class="['upload-text',  isFloatingBtn ? 'hidden-text' : '']" v-language:inner>
                        upload_files_component_share_study
                    </p>
                    <button round :class="['upload-btn',  isFloatingBtn ? 'rounded-floating-button' : '']">
                        <span class="btn-text" v-language:inner>upload_files_file_upload</span>
                        <v-icon class="sb-cloud-upload-icon" right>sbf-upload-cloud</v-icon>
                    </button>
                </div>
            </div>
        </a>
    </v-flex>
</template>

<script>
    import { mapActions, mapGetters } from 'vuex';

    export default {
        components: {},
        data() {
            return {
                offsetTop: 0,
                offsetTop2: 0,
            }
        },
        props: {
            isNotes: {
                type: Boolean,
                default: false,
                required: false
            }
        },
        computed: {
            ...mapGetters(['accountUser', 'loginDialogState', 'getSelectedClasses', 'getDialogState', 'showRegistrationBanner']),

            isFloatingBtn() {
                let offHeight = 0;
                if(this.showRegistrationBanner){
                    // offHeight = 150 + 285; // header + signUpBanner height
                    offHeight = 150 + 285;
                }else{
                    offHeight = 150;
                }
                return this.offsetTop2 >= offHeight && (this.$vuetify.breakpoint.smAndDown)
            },
            showUploadDialog() {
                return this.getDialogState
            },
        },

        methods: {
            ...mapActions([
                "updateLoginDialogState",
                'updateNewQuestionDialogState',
                'updateDialogState',
                'changeSelectPopUpUniState',
            ]),
            ...mapGetters(['getSchoolName']),
            openUploaderDialog() {
                let schoolName = this.getSchoolName();
                if (this.accountUser == null) {
                    this.updateLoginDialogState(true);
                } else if (schoolName.length > 0 && this.getSelectedClasses.length > 0) {
                    this.updateDialogState(true);
                } else {
                    this.changeSelectPopUpUniState(true)
                }
            },
            transformToBtn() {
                this.offsetTop2 = window.pageYOffset || document.documentElement.scrollTop;
            },
        },

        beforeMount: function () {
            if (window) {
                window.addEventListener('scroll', this.transformToBtn)
            }
        },
        beforeDestroy: function () {
            if (window) {
                window.removeEventListener('scroll', this.transformToBtn)
            }
        }
    }
</script>

<style scoped lang="less" src="./uploadFilesBtn.less"></style>