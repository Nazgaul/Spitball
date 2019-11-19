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
        <template slot="text" class="mb-4">
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
      <v-list-item
        v-for="singleLang in languageChoisesAval"
        :key="singleLang.name"
        @click="changeLanguage(singleLang.id)"
        sel="menu_row"
      >
        <v-list-item-action>
          <v-icon>{{singleLang.icon}}</v-icon>
        </v-list-item-action>
        <v-list-item-content>
          <v-list-item-title class="subtitle-1">{{singleLang.title}}</v-list-item-title>
        </v-list-item-content>
      </v-list-item>
      <!-- end language swith-->
      <v-list-item v-for="link in satelliteLinks" :key="link.title">
        <v-list-item-action>
          <v-icon>{{link.icon}}</v-icon>
        </v-list-item-action>
        <v-list-item-content>
          <a :href="link.url" class="v-list__tile__title subtitle-1">{{link.title}}</a>
        </v-list-item-content>
      </v-list-item>
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
      <!--<router-link tag="v-list-item" :to="{name:'wallet'}">-->
      <!--!!!!Use this instead, v-list tile with :to !!!!!-->
      <v-list-item :to="{name:'wallet'}" sel="menu_row">
        <v-list-item-action>
          <v-icon>sbf-wallet</v-icon>
        </v-list-item-action>
        <v-list-item-content>
          <v-list-item-title class="subtitle-1" v-language:inner>menuList_my_wallet</v-list-item-title>
        </v-list-item-content>
      </v-list-item>
      <v-list-item :to="{name:'profile',params:{id:accountUser.id,name:accountUser.name}}" sel="menu_row">
        <v-list-item-action>
          <v-icon>sbf-user</v-icon>
        </v-list-item-action>
        <v-list-item-content>
          <v-list-item-title class="subtitle-1" v-language:inner>menuList_my_profile</v-list-item-title>
        </v-list-item-content>
      </v-list-item>
      <v-list-item
        @click.native.prevent="openPersonalizeUniversity()"
        sel="menu_row"
        :to="{name: 'addUniversity'}"
      >
        <v-list-item-action>
          <v-icon>sbf-university</v-icon>
        </v-list-item-action>
        <v-list-item-content>
          <v-list-item-title class="subtitle-1" v-language:inner>menuList_changeUniversity</v-list-item-title>
        </v-list-item-content>
        <v-list-item-action>
          <span class="edit-text">
            <v-icon class="edit-after-icon">sbf-edit-icon</v-icon>
          </span>
        </v-list-item-action>
      </v-list-item>
      <v-list-item
        sel="menu_row"
        @click.native.prevent="openPersonalizeCourse()"
        :to="{name: 'editCourse'}">
        <v-list-item-action>
          <v-icon>sbf-classes</v-icon>
        </v-list-item-action>
        <v-list-item-content>
          <v-list-item-title class="subtitle-1" v-language:inner>menuList_changeCourse</v-list-item-title>
        </v-list-item-content>
        <v-list-item-action>
          <span class="edit-text">
            <v-icon class="edit-after-icon">sbf-edit-icon</v-icon>
          </span>
        </v-list-item-action>
      </v-list-item>

     
      <v-list-item
        :to="{ name: 'tutoring'}"
        target="_blank"
        sel="menu_row"
      >
        <v-list-item-action>
          <v-icon>sbf-studyroom-icon</v-icon>
        </v-list-item-action>
        <v-list-item-content>
          <v-list-item-title class="subtitle-1" v-language:inner>menuList_my_study_rooms</v-list-item-title>
        </v-list-item-content>
      </v-list-item>
      
      <template v-if="!isFrymo">
        <v-list-item
          v-for="singleLang in languageChoisesAval"
          :key="singleLang.name"
          sel="menu_row"
          @click="changeLanguage(singleLang.id)"
        >
          <v-list-item-action>
            <v-icon>{{singleLang.icon}}</v-icon>
          </v-list-item-action>
          <v-list-item-content>
            <v-list-item-title class="subtitle-1">{{singleLang.title}}</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
      </template>

      <v-list-item @click="startIntercom" sel="menu_row">
        <v-list-item-action>
          <v-icon>sbf-feedbackNew</v-icon>
        </v-list-item-action>
        <v-list-item-content>
          <v-list-item-title class="subtitle-1" v-language:inner>menuList_feedback</v-list-item-title>
        </v-list-item-content>
      </v-list-item>

      <v-list-item @click="logout" sel="menu_row">
        <v-list-item-action>
          <v-icon>sbf-logout</v-icon>
        </v-list-item-action>
        <v-list-item-content>
          <v-list-item-title class="subtitle-1" v-language:inner>menuList_logout</v-list-item-title>
        </v-list-item-content>
      </v-list-item>
      <v-divider class="my-3"></v-divider>
      <v-list-item @click="openReferralDialog" sel="menu_row">
        <v-list-item-action>
          <v-icon>sbf-user</v-icon>
        </v-list-item-action>
        <v-list-item-content>
          <v-list-item-title class="subtitle-1" v-language:inner>menuList_referral_spitball</v-list-item-title>
        </v-list-item-content>
      </v-list-item>

      <v-list-item v-for="link in satelliteLinks" :key="link.title" sel="menu_row">
        <v-list-item-action>
          <v-icon>{{link.icon}}</v-icon>
        </v-list-item-action>
        <v-list-item-content>
          <a :href="link.url" class="v-list__tile__title subtitle-1">{{link.title}}</a>
        </v-list-item-content>
      </v-list-item>
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
    ...mapGetters(["unreadMessages", "accountUser", "getSchoolName", 'isFrymo']),
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
      let site = this.isFrymo ? 'frymo.com' : 'spitball.co';
      return `http://www.${site}/?referral=${Base62.encode(this.accountUser.id)}&promo=referral`;
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
      return val ? "router-link" : "v-list-item";
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
      if(this.isFrymo){
        window.open('mailto: support@frymo.com', '_blank');
      }else{
        Intercom("showNewMessage");
      }
      
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