<template>
  <div>
    <v-list class="menu-list">
      <div class="userMenu_cont">
        <div class="userMenu_top_postion" v-if="isLoggedIn"/>
        <div class="userMenu_top_noLogin" v-if="!isLoggedIn && isMobile">
          <div class="userMenu_logo"><logoComponent/></div>
            <div class="uM_noLogin_txt" v-language:inner="'menuList_txt_out'"/>
            <div class="uM_noLogin_btns">
              <v-btn round depressed class="uM_noLogin_btns_in" color="white" :to="{ path: '/signin'}" v-language:inner="'menuList_Login'"/>
              <v-btn round depressed class="uM_noLogin_btns_up" color="#4c59ff" :to="{ name: 'registration'}" v-language:inner="'menuList_Sign_up'"/>
            </div>
        </div>
        <div class="userMenu_top" v-if="isLoggedIn">
          <userAvatar size="80" :userImageUrl="user.image" :user-name="user.name" :user-id="user.id"/>
          <div class="uM_top_txts">
            <h1 class="uM_title" v-html="$Ph('menuList_greets', user.name)"/>
            <h2 class="uM_subtitle" v-html="$Ph('menuList_balance', user.balance.toFixed())"/>
          </div>
        </div>
        <!-- <div class="userMenu_userList" v-if="isLoggedIn"> -->
          <!-- <v-list-tile @click="openSblToken">
            <v-list-tile-action>
              <getPointsSVG class="userMenu_icons"></getPointsSVG>
              </v-list-tile-action>
            <v-list-tile-content>
              <v-list-tile-title class="subheading userMenu_titles" v-language:inner="'menuList_points'"/>
              </v-list-tile-content>
          </v-list-tile> -->
          <!-- <v-list-tile @click.native.prevent="openPersonalizeCourse()" :to="{name: 'editCourse'}">
            <v-list-tile-action><classesIcon class="userMenu_icons"/></v-list-tile-action>
            <v-list-tile-content><v-list-tile-title class="subheading userMenu_titles" v-language:inner="'menuList_changeCourse'"/></v-list-tile-content>
          </v-list-tile>  -->
          <!-- <v-list-tile @click.native.prevent="openPersonalizeUniversity()" :to="{name: 'addUniversity'}">
            <v-list-tile-action><v-icon class="userMenu_icons" v-html="'sbf-university'"/></v-list-tile-action>
            <v-list-tile-content><v-list-tile-title class="subheading userMenu_titles" v-language:inner="'menuList_changeUniversity'"/></v-list-tile-content>
            <v-list-tile-action><span class="edit-text"><v-icon class="edit-after-icon" v-html="'sbf-edit-icon'"/></span></v-list-tile-action>
          </v-list-tile> -->
            <!-- <v-list-tile
              :to="{ name: 'tutoring'}"
              target="_blank"
              sel="menu_row"
            >
              <v-list-tile-action>
                <v-icon class="userMenu_icons">sbf-studyroom-icon</v-icon>
              </v-list-tile-action>
              <v-list-tile-content>
                <v-list-tile-title class="subheading userMenu_titles" v-language:inner>menuList_my_study_rooms</v-list-tile-title>
              </v-list-tile-content>
            </v-list-tile> -->
        <!-- </div> -->

        <div class="userMenu_actionsList">
          <v-list-tile v-if="!isFrymo" v-for="singleLang in languageChoisesAval" :key="singleLang.name" @click="changeLanguage(singleLang.id)" sel="menu_row">
            <v-list-tile-action><v-icon class="userMenu_icons">{{singleLang.icon}}</v-icon></v-list-tile-action>
            <v-list-tile-content><v-list-tile-title class="subheading userMenu_titles">{{singleLang.title}}</v-list-tile-title></v-list-tile-content>
          </v-list-tile>
          <v-list-tile v-for="link in satelliteLinks" :key="link.title" sel="menu_row">
            <v-list-tile-action>
              <a :href="link.url"><v-icon class="userMenu_icons">{{link.icon}}</v-icon></a>
            </v-list-tile-action>
            <v-list-tile-content>
              <a :href="link.url" class="v-list__tile__title subheading userMenu_titles">{{link.title}}</a>
            </v-list-tile-content>
          </v-list-tile>
         <v-list-tile @click="openReferralDialog" v-if="isLoggedIn" sel="menu_row">
            <v-list-tile-action>
                <v-icon class="userMenu_icons">sbf-user</v-icon>
            </v-list-tile-action>
            <v-list-tile-content>
                <v-list-tile-title class="subheading userMenu_titles" v-language:inner>menuList_referral_spitball</v-list-tile-title>
            </v-list-tile-content>
          </v-list-tile>
          <v-list-tile @click="startIntercom" v-if="isLoggedIn" sel="menu_row">
            <v-list-tile-action>
              <v-icon class="userMenu_icons">sbf-feedbackNew</v-icon>
            </v-list-tile-action>
            <v-list-tile-content>
              <v-list-tile-title class="subheading userMenu_titles" v-language:inner>menuList_feedback</v-list-tile-title>
            </v-list-tile-content>
          </v-list-tile>
                       <v-list-tile @click="logout" v-if="isLoggedIn" sel="menu_row">
            <v-list-tile-action>
              <v-icon class="userMenu_icons">sbf-logout</v-icon>
            </v-list-tile-action>
            <v-list-tile-content>
              <v-list-tile-title class="subheading userMenu_titles" v-language:inner>menuList_logout</v-list-tile-title>
            </v-list-tile-content>
          </v-list-tile>
        </div>
      </div>
    </v-list>
  </div>
</template>
<script>
import { mapGetters, mapActions } from "vuex";

import analyticsService from '../../../../services/analytics.service';
import languagesLocales from "../../../../services/language/localeLanguage";
import { LanguageChange, LanguageService } from "../../../../services/language/languageService";
import satelliteService from '../../../../services/satelliteService';

import userAvatar from '../../../helpers/UserAvatar/UserAvatar.vue';
import sbDialog from "../../../wrappers/sb-dialog/sb-dialog.vue";
import logoComponent from '../../../app/logo/logo.vue';

import getPointsSVG from './image/get-points.svg';
import classesIcon from './image/classes-icon.svg';

export default {
   components: { sbDialog,userAvatar,getPointsSVG,classesIcon,logoComponent},
  data() {
    return {
      languagesLocales,
      languageChoisesAval: [],
      satelliteLinks:[
{
             title: LanguageService.getValueByKey('menuList_help'),
             icon: 'sbf-help',
             url: satelliteService.getSatelliteUrlByName('faq')
         },
        {
             title: LanguageService.getValueByKey('menuList_about_spitball'), 
             icon: 'sbf-about',
             url: satelliteService.getSatelliteUrlByName('about')
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
  computed: {
    ...mapGetters(["accountUser",'isFrymo']),
    isMobile() {
      return this.$vuetify.breakpoint.smAndDown;
    },
    user() {
      return { ...this.accountUser };
    },
    isLoggedIn() {
      return !!this.accountUser;
    },
  },
  methods: {           
    ...mapActions(['updateReferralDialog','updateShowBuyDialog',"logout","updateLoginDialogState",]),
    // openSblToken(){
    //   this.$emit('closeMenu')

    //   analyticsService.sb_unitedEvent("BUY_POINTS", "ENTER");
    //   this.updateShowBuyDialog(true);
    // },  
    changeLanguage(id) {
      LanguageChange.setUserLanguage(id).then(
        resp => {
          global.location.reload(true);
        },
        error => {
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
      this.$emit('closeMenu')
      setTimeout(() => {
        this.updateReferralDialog(true);
      });
    },
    // openPersonalizeUniversity() {
    //   if (!this.isLoggedIn) {
    //     this.updateLoginDialogState(true);
    //   } else {
    //     this.$router.push({ name: "addUniversity" });
    //     this.$root.$emit("closeDrawer");
    //   }
    // },
    // openPersonalizeCourse() {
    //   if (!this.isLoggedIn) {
    //     this.updateLoginDialogState(true);
    //   } else {
    //     this.$router.push({ name: "editCourse" });
    //     this.$root.$emit("closeDrawer");
    //   }
    // },
  },

  created() {
    let currentLocHTML = document.documentElement.lang;
    this.languageChoisesAval = languagesLocales.filter(lan => {
      return lan.locale !== currentLocHTML;
    });
    // this.$root.$on('closePopUp', (name) => {
    //     if (name === "referralPop") {
    //         console.log('fsdf')
    //         this.updateReferralDialog(false)
    //     }
    // });
    if (
      !!this.$route.query &&
      !!this.$route.query.open &&
      this.$route.query.open === "referral"
    ) {
      this.$nextTick(function() {
        this.updateReferralDialog(true)
      });
    }
  }
};
</script>

<style src="./menuList.less" lang="less"/>