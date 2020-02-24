<template>
  <div>
    <v-list class="menu-list" dense>
      <div class="userMenu_cont">
        <div class="userMenu_top_postion" v-if="isLoggedIn"/>
        <div class="userMenu_top_noLogin" v-if="!isLoggedIn && isMobile">
          <div class="userMenu_logo"><logoComponent/></div>
            <div class="uM_noLogin_txt" v-language:inner="'menuList_txt_out'"/>
            <div class="uM_noLogin_btns">
              <v-btn rounded depressed class="uM_noLogin_btns_in" color="white" :to="{ name: 'login'}" v-language:inner="'menuList_Login'"/>
              <v-btn rounded depressed class="uM_noLogin_btns_up" color="#4c59ff" :to="{ name: 'register'}" v-language:inner="'menuList_Sign_up'"/>
            </div>
        </div>
        <div class="userMenu_top" v-if="isLoggedIn">
          <userAvatar size="80" :userImageUrl="user.image" :user-name="user.name" :user-id="user.id"/>
          <div class="uM_top_txts">
            <h1 class="uM_title" v-html="$Ph('menuList_greets', user.name)"/>
            <h2 class="uM_subtitle" v-html="$Ph('menuList_balance', userBalance(user.balance))"/>
          </div>
        </div>
        <div class="userMenu_actionsList">
          <template v-if="showChangeLanguage">
            <v-list-item v-for="singleLang in languageChoisesAval" :key="singleLang.name" @click="changeLanguage(singleLang.id)" sel="menu_row">
              <v-list-item-action><v-icon class="userMenu_icons">{{singleLang.icon}}</v-icon></v-list-item-action>
              <v-list-item-content><v-list-item-title class="subheading userMenu_titles">{{singleLang.title}}</v-list-item-title></v-list-item-content>
            </v-list-item>
          </template>
        <template v-if="showFindTutors">
          <v-list-item :to="{name:'tutorLandingPage'}">
            <v-list-item-action><v-icon class="userMenu_icons" v-html="'sbf-account-group'"></v-icon></v-list-item-action>
            <v-list-item-content>
              <v-list-item-title><span class="userMenu_titles" v-language:inner="'header_find_tutors'"></span></v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </template>
        <template v-if="showTeachOnSpitball">
          <v-list-item href="https://teach.spitball.co/" target="_blank">
            <v-list-item-action>
              <v-icon v-html="'sbf-find'" class="userMenu_icons"/>
            </v-list-item-action>
            <v-list-item-content>
              <v-list-item-title><span class="userMenu_titles" v-language:inner="'profile_become_title'"></span></v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </template>
        <template v-if="showTestStudyRoom">
          <v-list-item :to="{ name: 'tutoring'}">
            <v-list-item-action><v-icon class="userMenu_icons" v-html="'sbf-pc'"></v-icon></v-list-item-action>
            <v-list-item-content>
              <v-list-item-title><span class="userMenu_titles" v-language:inner="'menuList_my_study_rooms'"></span></v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </template>
          
          <v-list-item v-for="link in satelliteLinks" :key="link.title" sel="menu_row">
            <v-list-item-action>
              <a :href="link.url"><v-icon class="userMenu_icons">{{link.icon}}</v-icon></a>
            </v-list-item-action>
            <v-list-item-content>
              <a :href="link.url" class="v-list__tile__title subheading userMenu_titles">{{link.title}}</a>
            </v-list-item-content>
          </v-list-item>
         <v-list-item @click="openReferralDialog" v-if="isLoggedIn" sel="menu_row">
            <v-list-item-action>
                <v-icon class="userMenu_icons">sbf-user</v-icon>
            </v-list-item-action>
            <v-list-item-content>
                <v-list-item-title class="subheading userMenu_titles" v-language:inner>menuList_referral_spitball</v-list-item-title>
            </v-list-item-content>
          </v-list-item>
          <v-list-item @click="startIntercom" v-if="isLoggedIn" sel="menu_row">
            <v-list-item-action>
              <v-icon class="userMenu_icons">sbf-feedbackNew</v-icon>
            </v-list-item-action>
            <v-list-item-content>
              <v-list-item-title class="subheading userMenu_titles" v-language:inner>menuList_feedback</v-list-item-title>
            </v-list-item-content>
          </v-list-item>
                       <v-list-item @click="logout" v-if="isLoggedIn" sel="menu_row">
            <v-list-item-action>
              <v-icon class="userMenu_icons">sbf-logout</v-icon>
            </v-list-item-action>
            <v-list-item-content>
              <v-list-item-title class="subheading userMenu_titles" v-language:inner>menuList_logout</v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </div>
      </div>
    </v-list>
  </div>
</template>
<script>
import { mapGetters, mapActions } from "vuex";

import languagesLocales from "../../../../services/language/localeLanguage";
import { LanguageChange, LanguageService } from "../../../../services/language/languageService";
import satelliteService from '../../../../services/satelliteService';

import logoComponent from '../../../app/logo/logo.vue';
import intercomSettings from '../../../../services/intercomService';

export default {
   components: {logoComponent},
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
    ...mapGetters(["accountUser",'isFrymo','getUserLoggedInStatus2']),
    isMobile() {
      return this.$vuetify.breakpoint.smAndDown;
    },
    user() {
      return { ...this.accountUser };
    },
    isLoggedIn() {
      return !!this.accountUser;
    },
    showChangeLanguage() {
      return global.country === 'IL';
    },
    showFindTutors(){
      if(this.$route.name === 'tutorLandingPage'){
        return false;
      }else{
        if(this.getUserLoggedInStatus2){
          return !this.accountUser.isTutor;
        }else{
          return true;
        }
      }
    },
    showTeachOnSpitball(){
      if(this.isFrymo){
        return false;
      }else{
        if(this.getUserLoggedInStatus2){
          return !this.accountUser.isTutor;
        }else{
          return true;
        }
      }
    },
    showTestStudyRoom(){
      if(this.getUserLoggedInStatus2){
        return !this.accountUser.isTutor;
      }else{
        return true;
      }
    }
  },
  methods: {           
    ...mapActions(['updateReferralDialog',"logout",]),
    changeLanguage(id) {
      LanguageChange.setUserLanguage(id).then(
        () => {
          global.location.reload(true);
        }     
      );
    },
    startIntercom() {
      intercomSettings.showDialog();
    },
    openReferralDialog() {
      this.$emit('closeMenu')
      setTimeout(() => {
        this.updateReferralDialog(true);
      });
    },
    userBalance(balance){
      let balanceFixed = +balance.toFixed()
      return balanceFixed.toLocaleString(`${global.lang}`)
    }
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