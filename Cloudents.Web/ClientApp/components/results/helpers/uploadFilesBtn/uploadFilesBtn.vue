<template>
    <v-flex :class="['result-cell', $vuetify.breakpoint.smAndUp ? 'mb-3': '', 'upload-files-action-card', 'xs-12', isFloatingBtn ? 'floatingcard' : 'regularCard']">
        <a class="mb-5 upload-link">
            <div :class="['upload-wrap', isFloatingBtn ? 'floating-upload' : '']">
                <div class="static-center">
                    <!--<p v-show="$vuetify.breakpoint.smAndUp"  :class="['upload-text',  isFloatingBtn ? 'hidden-text' : '']" v-language:inner>-->
                    <!--upload_files_component_share_study-->
                    <!--</p>-->
                    <button round
                            :class="['upload-btn',  isFloatingBtn ? 'rounded-floating-button' : '', {'raised': raiseFloatingButtonPosition}]"
                            @click="openUploaderDialog()">
                        <v-icon class="sb-cloud-upload-icon">sbf-upload-cloud</v-icon>
                        <span class="btn-text" v-language:inner>upload_files_file_upload</span>
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
            };
        },
        props: {
            isNotes: {
                type: Boolean,
                default: false,
                required: false
            }
        },
        computed: {
            ...mapGetters(['accountUser', 'loginDialogState', 'getSelectedClasses', 'getDialogState', 'getCookieAccepted']),

            isFloatingBtn() {
                if(this.$vuetify.breakpoint.smAndDown) {

                    return true;
                } else {
                    return false;
                }
                return this.offsetTop2 >= offHeight && (this.$vuetify.breakpoint.smAndDown);
            },
            raiseFloatingButtonPosition() {
                return !this.getCookieAccepted;
            },
            showUploadDialog() {
                return this.getDialogState;
            },
        },

        methods: {
            ...mapActions([
                              "updateLoginDialogState",
                              'updateDialogState',
                              'setReturnToUpload'
                          ]),
            ...mapGetters(['getSchoolName', 'getAllSteps']),
            openUploaderDialog() {
                let schoolName = this.getSchoolName();
                let steps = this.getAllSteps();
                if(this.accountUser == null) {
                    this.updateLoginDialogState(true);
                } else if(!schoolName.length) {
                    this.$router.push({name: 'addUniversity'});
                    this.setReturnToUpload(true);
                } else if(!this.getSelectedClasses.length) {
                    this.$router.push({name: 'addCourse'});
                    this.setReturnToUpload(true);
                } else if(schoolName.length > 0 && this.getSelectedClasses.length > 0) {
                    this.updateDialogState(true);
                    this.setReturnToUpload(false);
                }
            },
            transformToBtn() {
                this.offsetTop2 = window.pageYOffset || document.documentElement.scrollTop;
            },
        },

        beforeMount: function () {
            if(window) {
                window.addEventListener('scroll', this.transformToBtn);
            }
        },
        beforeDestroy: function () {
            if(window) {
                window.removeEventListener('scroll', this.transformToBtn);
            }
        }
    };
</script>

<style scoped lang="less" src="./uploadFilesBtn.less"></style>