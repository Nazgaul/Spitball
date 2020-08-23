<template>
  <v-navigation-drawer class="profileDrawer" v-model="drawer" :right="$vuetify.rtl" :permanent="drawer" :width="$vuetify.breakpoint.xsOnly ? '100%' : '338'" app fixed touchless clipped>
	<div class="drawerHeader">
		<div class="pa-5" v-t="'edit'"></div>
	</div>

    <v-form v-model="valid" ref="formTutor" class="pa-4">
        <!-- <div class="header d-flex justify-space-between align-start mb-8"> -->
            <!-- <div class="mainTitle" v-t="'upload images'"></div> -->
            <!-- <v-icon class="closeIcon" size="12" @click="closeDialog">{{$vuetify.icons.values.close}}</v-icon> -->
        <!-- </div> -->
        <div class="profilePicture mb-5 mt-2">
            <div class="pictureTitle mb-3" v-t="'profile picture'"></div>
            <div class="profileEditAvatarWrap">
                <uploadImage
                    sel="photo"
                    class="editImage"
                    v-show="avatarLoading || !$store.getters.getAccountImage"
                    @setProfileAvatarLoading="val => avatarLoading = val"
                />
                <userAvatarNew
                    sel="avatar_image"
                    class="pUb_dS_img"
                    :userName="$store.getters.getAccountName"
                    :userImageUrl="$store.getters.getAccountImage"
                    :width="134"
                    :height="164"
                    :userId="$store.getters.getAccountId"
                    :fontSize="36"
                    :borderRadius="8"
                    :tile="true"
                    :loading="avatarLoading"
                    @setAvatarLoaded="val => avatarLoading = val"
                />
            </div>
            <div class="avatrRecommended mt-0" v-t="'profile_pic_recommended'"></div>
        </div>

        <label class="profileCover mb-12" @click="$vuetify.goTo('#profileCover')">
            <div class="coverTitle mt-8 mb-3" v-t="'profile cover'"></div>
            <cover  />
            <div class="coverRecommended text-center mt-2" v-t="'profile_cover_recommended'"></div>
        </label>

        <v-row no-gutters class="profileInfo">
            <v-col cols="12" class="my-4">
                <div class="infoTitle mt-8" v-t="'profile_personal_details'"></div>
            </v-col>
            <v-col cols="12" sm="6">
                <v-text-field
                    class="me-sm-5"
                    :rules="[rules.required, rules.minimumChars]"
                    :label="$t('profile_firstName_label')"
                    dense
                    height="44"
                    v-model.trim="firstName"
                    outlined
                ></v-text-field>
            </v-col>
            <v-col cols="12" sm="6">
                <v-text-field
                    v-model.trim="lastName"
                    :rules="[rules.required, rules.minimumChars]"
                    :label="$t('profile_lastName_label')"
                    dense
                    height="44"
                    outlined
                ></v-text-field>
            </v-col>
            <v-col cols="12">
                <v-textarea
                    rows="2"
                    class="my-1"
                    outlined
                    v-model="title" 
                    :rules="[rules.titleMaxChars]"
                    @focus="$vuetify.goTo('#profileCover')"
                    :counter="TITLE_MAX"
                    :label="$t('profile_description_label')"
                    dense
                    height="44"
                    :placeholder="$t('title placeholder')"
                ></v-textarea>
            </v-col>
            <v-col cols="12">
                <v-textarea
                    rows="2"
                    outlined
                    class="mb-1"
                    v-model="shortParagraph"
                    @focus="$vuetify.goTo('#profileCover')"
                    :rules="[rules.shortParagraphMaxChars]"
                    :counter="SHORTPARAGRAPG_MAX"
                    :label="$t('Short paragraph')"
                    :placeholder="$t('shortParagraph placeholder')"
                ></v-textarea>
            </v-col>
            <v-col>
                <v-textarea
                    rows="5"
                    outlined
                    v-model="bio"
                    class="tutor-edit-bio"
                    @focus="$vuetify.goTo('#profileParagraph')"
                    :label="$t('profile_bio_label')"
                    :placeholder="$t('bio placeholder')"
                ></v-textarea>
            </v-col>
        </v-row>
        
        <profileCourses />

        <div class="text-center profileDrawerSticky">
            <v-btn :disabled="btnLoading" width="120" depressed color="#4452fc" class="shallow-blue ms-0" rounded outlined primary @click="closeDialog">
                <span v-t="'cancel'"></span>
            </v-btn>
            <v-btn class="blue-btn white--text ms-4" width="120" depressed color="#4452fc" rounded @click="saveChanges" :loading="btnLoading">
                <span v-t="'save'"></span>
            </v-btn>
        </div>
    </v-form>


	<!-- <div class="text-center pt-9">
		<v-btn class="me-3" color="#4452fc" width="132" height="40" v-t="'cancel'" rounded depressed outlined/>
		<v-btn class="white--text" color="#4452fc" width="132" height="40" v-t="'save'" depressed rounded/>
	</div> -->
  </v-navigation-drawer>
</template>

<script>
import { validationRules } from '../../../services/utilities/formValidationRules';
import profileCourses from './profileCourse.vue';
import uploadImage from '../profileHelpers/profileBio/bioParts/uploadImage/uploadImage.vue'
import cover from '../components/cover.vue'

export default {
    name: "tutorInfoEdit",
    components: {
        profileCourses,
        uploadImage,
        cover
    },
    data() {
        return {
            TITLE_MAX: 52,
            SHORTPARAGRAPG_MAX: 100,
            editedBio: '',
            editTitle: '',
            editShortParagraph: '',
            editedFirstName: '',
            editedLastName: '',
            valid: false,
            btnLoading: false,
            avatarLoading: false,
            rules: {
                required: (value) => validationRules.required(value),
                minimum: (value) => validationRules.minimumChars(value, 10),
                maximum: (value) => validationRules.maxVal(value, 1000),
                maximumChars: (value) => validationRules.maximumChars(value, 1000),
                minimumChars: (value) => validationRules.minimumChars(value, 2),
                titleMaxChars: (value) => validationRules.maximumChars(value, this.TITLE_MAX),
                shortParagraphMaxChars: (value) => validationRules.maximumChars(value, this.SHORTPARAGRAPG_MAX)
            }
        };
    },
    computed: {
        drawer: {
            get() {
                return this.$store.getters.getProfileCoverDrawer
            },
            set(newVal) {
                this.$store.commit('setToggleProfileDrawer', newVal)
            }
        },
        broadcastSessions() {
            return this.$store.getters.getProfileCourses;
        },
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly
        },
        shortParagraph: {
            get() {
                return this.$store.getters.getProfileBio
            },
            set(newVal) {
                this.editShortParagraph = newVal;
                if(newVal.length <= this.SHORTPARAGRAPG_MAX) {
                    this.$store.commit('setFakeShorParagraph', newVal)
                }
            }
        },
        bio: {
            get() {
                return this.$store.getters.getProfileParagraph
            },
            set(newVal) {
                this.editedBio = newVal;
                this.$store.commit('setFakeBio', newVal)
            }
        },
        firstName: {
            get() {
                return this.$store.getters.getProfileFirstName;
            },
            set(newVal) {
                this.editedFirstName = newVal;
            }
        },
        lastName: {
            get() {
                return this.$store.getters.getProfileLastName;
            },
            set(newVal) {
                this.editedLastName = newVal;
            }
        },
        title: {
            get() {
                return this.$store.getters.getProfileTitle;
            },
            set(newVal) {
                this.editTitle = newVal;
                if(newVal.length <= this.TITLE_MAX) {
                    this.$store.commit('setFakeShortTitle', newVal)
                }
            }
        }
    },
    methods: {
        saveChanges() {
            if(this.$refs.formTutor.validate()) {
                this.btnLoading = true;
                let serverFormat = {
                    firstName: this.editedFirstName ||  this.firstName,
                    lastName: this.editedLastName || this.lastName,
                    shortParagraph: this.editShortParagraph || undefined, // this.shortParagraph,
                    bio: this.editedBio || undefined, // this.bio,
                    title: this.editTitle || undefined // this.title
                };
                this.$store.dispatch('saveUserInfo', serverFormat)
                    .then(() => {
                        this.closeDialog();
                    }).catch(ex => {
                        console.error(ex);
                    }).finally(() => {
                        this.btnLoading = false;
                    });
            }
        },
        closeDialog() {
            this.$store.commit('setToggleProfileDrawer', false)
            this.$store.commit('setFakeShorParagraph', '')
            this.$store.commit('setFakeBio', '')
            this.$store.commit('setFakeShortTitle', '')
            this.editedBio = ''
            this.editTitle = ''
            this.editShortParagraph = ''
            this.editedFirstName = ''
            this.editedLastName = ''
        }
    }
};
</script>

<style lang="less">
@import '../../../styles/mixin.less';
.profileDrawer {
    .drawerHeader {
        border-bottom: solid 1px #dddddd;
        font-size: 20px;
        font-weight: 600;
        line-height: 1.15;
        color: #43425d;
    }
    background: #fff;
    @media (max-width: @screen-xs) {
        overflow-x: hidden;
    }
    .header {
        .mainTitle {
            .responsive-property(font-size, 22px, null, 20px);
            font-weight: 600;
            color: #43425d;
        }
    }
    .profilePicture {
        position: relative;
        @media (max-width: @screen-xs) {
            padding: 8px 6px;
            background: #fff;
            border-radius: 8px;
        }
        .pictureTitle {
            font-size: 16px;
            font-weight: 600;
            color: #131415;
        }
        .profileEditAvatarWrap {
            width: max-content;
            // margin: 0 auto;
            .pUb_dS_img{
                pointer-events: none !important;
            }
            .editImage{
                position: absolute;
                // right: 4px;
                text-align: center;
                // width: 36px;
                // height: 46px;
                border-radius: 3px;
                background-color: rgba(0,0,0,.6);
                z-index: 1;
            }
            .user-avatar-image-wrap {
                // width: auto !important;
                .user-avatar-rect-img {
                    border: solid 1px #c6cdda;
                    border-radius: 3px !important;
                }
            }
        }
        .avatrRecommended {
            font-size: 12px;
            color: #a4a7ab;
        }
    }
    .profileCover {
        cursor: pointer;
        position: relative;
        .coverTitle {
            font-size: 16px;
            font-weight: 600;
            color: #43425d;
        }
        .coverPhoto {
            object-fit: cover;
            height: 150px;
            border-radius: 3px;
            border: solid 1px #c6cdda;
        }
        .coverupload {
            background: rgba(0,0,0,.6)
        }
        .imageLinear {
            display: none;
        }
        .coverRecommended {
            font-size: 12px;
            color: #a4a7ab;
        }
    }
    .profileInfo {
        .infoTitle {
            font-size: 16px;
            font-weight: 600;
            color: #43425d;
        }
        .v-textarea, .v-input {
            .v-input__slot {
                fieldset {
                    border: 1px solid #b8c0d1;
                }
                .v-label {
                    color: @global-purple;
                }
            }
            &.error--text {
                fieldset {
                    border: 2px solid #ff5252;
                }
            }
        }
        textarea {
            resize: none;
            margin-top: 10px !important;
            &::placeholder {
                color: #a4a7ab;
                font-size: 14px;
            }
        }
    }
    .profileDrawerSticky {
        position: sticky;
        bottom: 0;
        background: white;
        padding: 10px;
    }
}
</style>