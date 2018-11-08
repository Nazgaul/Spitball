<template>
    <v-flex  :class="['result-cell', 'mb-3', 'upload-files-action-card', 'xs-12', isFloatingBtn ? 'floatingcard' : 'regularCard']"
             v-scroll="transformToBtn()">
        <a class="mb-5 upload-link" @click="openUploaderDialog()">
            <div :class="['upload-wrap', $vuetify.breakpoint.smAndDown  ? 'mobile' :  'desktop', isFloatingBtn ? 'floating-upload' : '']">
                <div class="static-center">
                    <p class="upload-text" v-language:inner>upload_files_component_share_study
                    </p>
                    <v-btn round :class="['upload-btn',  isFloatingBtn ? 'rounded-floating-button' : '']">
                        <span>Upload a file</span>
                        <v-icon class="sb-cloud-upload-icon" right>sbf-upload-cloud</v-icon>
                    </v-btn>
                </div>
            </div>
        </a>
    </v-flex>
</template>

<script>
    import {mapActions, mapGetters} from 'vuex';
    export default {
        components: {
        },
        data() {
            return {
                offsetTop: 0,
                offsetTop2: 0,
            }
        },
        props: {
            userName: {},
            isNotes: {
                type: Boolean,
                default: false,
                required: false
            }
        },
        computed: {
            ...mapGetters(['accountUser', 'loginDialogState', 'getSelectedClasses', 'getDialogState']),
            isHiddenBlock(){
                return this.offsetTop >= 75 && (this.$vuetify.breakpoint.name === 'xs' && 'sm')

            },
            isFloatingBtn(){
                console.log('floating',this.offsetTop2 >= 150 && (this.$vuetify.breakpoint.name === 'xs' && 'sm'));
                return this.offsetTop2 >= 150 && (this.$vuetify.breakpoint.name === 'xs' && 'sm')
            },
            showUploadDialog() {
                return this.getDialogState
            },
        },
        methods: {
            ...mapActions([
                "updateLoginDialogState",
                'updateUserProfileData',
                'updateNewQuestionDialogState',
                'updateDialogState',
                'changeSelectPopUpUniState',
                'syncUniData'
            ]),
            openUploaderDialog() {
                if (this.accountUser == null) {
                    this.updateLoginDialogState(true);
                } else if (this.accountUser.universityExists && this.getSelectedClasses.length > 0) {
                    this.updateDialogState(true);
                } else {
                    this.changeSelectPopUpUniState(true)
                }

            },

            hideOnMobileScroll(e) {
                this.offsetTop = window.pageYOffset || document.documentElement.scrollTop;
            },
            transformToBtn(){
                console.log('floatin', this.isFloatingBtn)
                this.offsetTop2 = window.pageYOffset || document.documentElement.scrollTop;

            },

        },
        created(){
            this.syncUniData()
            console.log('created upload button componnet uni class sync')

        }
    }
</script>

<style  scoped lang="less" src="./uploadFilesBtn.less"></style>