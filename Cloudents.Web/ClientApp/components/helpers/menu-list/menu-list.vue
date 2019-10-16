<template>
  <div>
    <v-list class="menu-list" v-if="!isAuthUser" content-class="s-menu-item">
      <user-block
        :user="user"
        :showExtended="true"
        :classType="'university'"
        v-if="isMobile"
        class="unsign"
      >
        <template slot="text" class="mb-3">
          <div class="menu-list-head-block">
            <div class="head-text-wrap">
              <h4 class="text--title" v-language:inner>menuList_Please</h4>
              <span class="mb-4 text" v-language:inner>menuList_sign_up_l</span>&nbsp;
              <span class="or" v-language:inner>menuList_or</span>&nbsp;
              <span class="mb-4 text" v-language:inner>menuList_login_l</span>&nbsp;
            </div>
            <div class="btn-container">
              <router-link
                class="login-btns body-1"
                :to="{ name: 'registration'}"
                v-language:inner
              >menuList_Sign_up</router-link>
              <router-link
                class="login-btns body-1"
                :to="{ path: '/signin'}"
                v-language:inner
              >menuList_Login</router-link>
            </div>
          </div>
        </template>
      </user-block>
      <!-- start language swith-->
      <v-list-tile
        :to="{ name: 'tutoring'}"
        target="_blank"
      >
        <v-list-tile-action>
          <v-icon>sbf-studyroom-icon</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title class="subheading" v-language:inner>menuList_my_study_rooms</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>
      <v-list-tile
        v-for="singleLang in languageChoisesAval"
        :key="singleLang.name"
        @click="changeLanguage(singleLang.id)"
      >
        <v-list-tile-action>
          <v-icon>{{singleLang.icon}}</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title class="subheading">{{singleLang.title}}</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>
      <!-- end language swith-->
      <template v-for="(item) in notRegMenu">
        <template v-if="item.name && item.name !== 'feedback'">
          <router-link tag="v-list-tile" :to="{name : item.name}">
            <v-list-tile-action>
              <v-icon>{{item.icon}}</v-icon>
            </v-list-tile-action>
            <v-list-tile-content>
              <v-list-tile-title class="subheading">{{item.title}}</v-list-tile-title>
            </v-list-tile-content>
          </router-link>
        </template>
        <!--if theres is click handler as in feedback/ check settings/const.js -->
        <v-list-tile
          v-else-if="item.name === 'feedback' && accountUser"
          @click="() => item.click ? item.click() : ''"
        >
          <v-list-tile-action>
            <v-icon>{{item.icon}}</v-icon>
          </v-list-tile-action>
          <v-list-tile-content>
            <v-list-tile-title class="subheading">{{item.title}}</v-list-tile-title>
          </v-list-tile-content>
        </v-list-tile>
      </template>
    </v-list>
    <!--mobile side menu open template-->
    <v-list class="menu-list" v-else>
      <user-block
        :user="user"
        :showExtended="true"
        :classType="'university authenticated-user'"
        v-if="isMobile"
      ></user-block>
      <!--!!!this wont generate link in dom!!!-->
      <!--<router-link tag="v-list-tile" :to="{name:'wallet'}">-->
      <!--!!!!Use this instead, v-list tile with :to !!!!!-->
      <v-list-tile :to="{name:'wallet'}">
        <v-list-tile-action>
          <v-icon>sbf-wallet</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title class="subheading" v-language:inner>menuList_my_wallet</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>
      <v-list-tile :to="{name:'profile',params:{id:accountUser.id,name:accountUser.name}}">
        <v-list-tile-action>
          <v-icon>sbf-user</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title class="subheading" v-language:inner>menuList_my_profile</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>
      <v-list-tile
        @click.native.prevent="openPersonalizeUniversity()"
        :to="{name: 'addUniversity'}"
      >
        <v-list-tile-action>
          <v-icon>sbf-university</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title class="subheading" v-language:inner>menuList_changeUniversity</v-list-tile-title>
        </v-list-tile-content>
        <v-list-tile-action>
          <span class="edit-text">
            <v-icon class="edit-after-icon">sbf-edit-icon</v-icon>
          </span>
        </v-list-tile-action>
      </v-list-tile>
      <v-list-tile
        @click.native.prevent="openPersonalizeCourse()"
        :to="{name: 'editCourse'}">
        <v-list-tile-action>
          <v-icon>sbf-classes</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title class="subheading" v-language:inner>menuList_changeCourse</v-list-tile-title>
        </v-list-tile-content>
        <v-list-tile-action>
          <span class="edit-text">
            <v-icon class="edit-after-icon">sbf-edit-icon</v-icon>
          </span>
        </v-list-tile-action>
      </v-list-tile>

     
      <v-list-tile
        :to="{ name: 'tutoring'}"
        target="_blank"
      >
        <v-list-tile-action>
          <v-icon>sbf-studyroom-icon</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title class="subheading" v-language:inner>menuList_my_study_rooms</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>

      <v-list-tile
        v-if="fromIL"
        v-for="singleLang in languageChoisesAval"
        :key="singleLang.name"
        @click="changeLanguage(singleLang.id)"
      >
        <v-list-tile-action>
          <v-icon>{{singleLang.icon}}</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title class="subheading">{{singleLang.title}}</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>

      <v-list-tile @click="startIntercom">
        <v-list-tile-action>
          <v-icon>sbf-feedbackNew</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title class="subheading" v-language:inner>menuList_feedback</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>

      <v-list-tile @click="logout">
        <v-list-tile-action>
          <v-icon>sbf-logout</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title class="subheading" v-language:inner>menuList_logout</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>
      <v-divider class="my-3"></v-divider>
      <v-list-tile @click="openReferralDialog">
        <v-list-tile-action>
          <v-icon>sbf-user</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title class="subheading" v-language:inner>menuList_referral_spitball</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>

      <v-list-tile v-for="link in satelliteLinks" :key="link.title">
        <v-list-tile-action>
          <v-icon>{{link.icon}}</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <a :href="link.url" class="v-list__tile__title subheading">{{link.title}}</a>
        </v-list-tile-content>
      </v-list-tile>
    </v-list>

    <sb-dialog
      v-if="isLoggedIn"
      :showDialog="showReferral"
      :popUpType="'referralPop'"
      :onclosefn="closeReferralDialog"
      :content-class="'login-popup'"
    >
      <referral-dialog
        :isTransparent="true"
        :onclosefn="closeReferralDialog"
        :showDialog="showReferral"
        :userReferralLink="userReferralLink"
        :popUpType="'referralPop'"
      ></referral-dialog>
    </sb-dialog>
  </div>
</template>
<script>
import { mapGetters, mapActions } from "vuex";
import { notRegMenu } from "../../settings/consts";
import userBlock from "../user-block/user-block.vue";
import sbDialog from "../../wrappers/sb-dialog/sb-dialog.vue";
import referralDialog from "../../question/helpers/referralDialog/referral-dialog.vue";
import languagesLocales from "../../../services/language/localeLanguage";
import { LanguageChange, LanguageService } from "../../../services/language/languageService";
import satelliteService from '../../../services/satelliteService';
import Base62 from "base62";

export default {
  components: { userBlock, sbDialog, referralDialog },
  data() {
    return {
      notRegMenu,
      showSettingsFirst: false,
      showSettings: false,
      showReferral: false,
      languagesLocales,
      languageChoisesAval: [],
      fromIL: global.country.toLowerCase() === "il",
      satelliteLinks:[
        {
             title: LanguageService.getValueByKey('menuList_about_spitball'), 
             icon: 'sbf-about',
             url: satelliteService.getSatelliteUrlByName('about')
         },
         {
             title: LanguageService.getValueByKey('menuList_help'),
             icon: 'sbf-help',
             url: satelliteService.getSatelliteUrlByName('faq')
         },
         {
             title: LanguageService.getValueByKey('menuList_terms_of_service'),
             icon: 'sbf-terms',
             url: satelliteService.getSatelliteUrlByName('terms') 
         },
         {
             title: LanguageService.getValueByKey('menuList_privacy_policy'),
             icon: 'sbf-privacy',
             url: satelliteService.getSatelliteUrlByName('privacy') 
         },
                    
      ]
    };
  },
  props: {
    counter: {
      required: false,
      type: Object
    },
    isAuthUser: {
      type: Boolean,
      default: false
    }
  },
  computed: {
    ...mapGetters(["unreadMessages", "accountUser", "getSchoolName"]),
    isMobile() {
      return this.$vuetify.breakpoint.width < 1024;
    },
    user() {
      return { ...this.accountUser };
    },
    isLoggedIn() {
      return !!this.accountUser;
    },
    userReferralLink() {
      return (
        "http://www.spitball.co/" +
        "?referral=" +
        Base62.encode(this.accountUser.id) +
        "&promo=referral"
      );
    }
  },
  watch: {
    showReferral(newValue, oldValue) {
      console.log(newValue, oldValue);
      console.trace(this.showReferral);
    }
  },
  methods: {
    ...mapActions([
      "logout",
      "updateLoginDialogState",
      "changeSelectUniState",
      "updateCurrentStep"
    ]),
    currentTemplate(val) {
      return val ? "router-link" : "v-list-tile";
    },
    changeLanguage(id) {
      LanguageChange.setUserLanguage(id).then(
        resp => {
          console.log("language responce success", resp);
          global.location.reload(true);
        },
        error => {
          console.log("language error error", error);
        }
      );
    },
    startIntercom() {
      Intercom("showNewMessage");
    },
    openReferralDialog() {
      setTimeout(() => {
        this.showReferral = true;
        console.log("referral", this.showReferral);
      });
    },
    openPersonalizeUniversity() {
      if (!this.isLoggedIn) {
        this.updateLoginDialogState(true);
      } else {
        this.$router.push({ name: "addUniversity" });
        this.$root.$emit("closeDrawer");
      }
    },
    openPersonalizeCourse() {
      if (!this.isLoggedIn) {
        this.updateLoginDialogState(true);
      } else {
        this.$router.push({ name: "editCourse" });
        this.$root.$emit("closeDrawer");
      }
    },
    closeReferralDialog() {
      this.showReferral = false;
    },
  },

  created() {
    // filter out cuurent language, to show in menu avaliable
    let currentLocHTML = document.documentElement.lang;
    this.languageChoisesAval = languagesLocales.filter(lan => {
      return lan.locale !== currentLocHTML;
    });
    // this.$root.$on('closePopUp', (name) => {
    //     if (name === "referralPop") {
    //         console.log('fsdf')
    //         this.showReferral = false;
    //     }
    // });
    if (
      !!this.$route.query &&
      !!this.$route.query.open &&
      this.$route.query.open === "referral"
    ) {
      this.$nextTick(function() {
        this.showReferral = true;
      });
    }
  }
};
</script>

<style src="./menu-list.less" lang="less"></style>