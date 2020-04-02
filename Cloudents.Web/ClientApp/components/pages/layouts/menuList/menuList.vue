<template>
  <div>
    <v-list class="menu-list" dense>
      <div class="userMenu_cont">
        <div class="userMenu_top_postion" v-if="isLoggedIn"/>
        <div class="userMenu_top_noLogin" v-if="!isLoggedIn && isMobile">
            <div class="userMenu_logo"><logoComponent/></div>
            <div class="uM_noLogin_txt">{{$t('menuList_txt_out')}}</div>
            <div class="uM_noLogin_btns">
              <v-btn rounded depressed class="uM_noLogin_btns_in" color="white" @click="openLoginDialog">{{$t('menuList_Login')}}</v-btn>
              <v-btn rounded depressed class="uM_noLogin_btns_up" color="#4c59ff" @click="openRegisterTypeDialog">{{$t('menuList_Sign_up')}}</v-btn>
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

        <template v-if="!getIsStudent && !getUserLoggedInStatus">
          <template>
            <v-list-item :to="{name:'tutorLandingPage'}">
              <v-list-item-action><v-icon class="userMenu_icons" v-html="'sbf-account-group'"></v-icon></v-list-item-action>
              <v-list-item-content>
                <v-list-item-title><span class="userMenu_titles">{{$t('header_find_tutors')}}</span></v-list-item-title>
              </v-list-item-content>
            </v-list-item>
          </template>
          <template v-if="!isFrymo">
            <v-list-item href="https://teach.spitball.co/" target="_blank">
              <v-list-item-action>
                <v-icon v-html="'sbf-find'" class="userMenu_icons"/>
              </v-list-item-action>
              <v-list-item-content>
                <v-list-item-title><span class="userMenu_titles">{{$t('profile_become_title')}}</span></v-list-item-title>
              </v-list-item-content>
            </v-list-item>
          </template>
          <template>
            <v-list-item :to="{ name: 'tutoring'}">
              <v-list-item-action><v-icon class="userMenu_icons" v-html="'sbf-pc'"></v-icon></v-list-item-action>
              <v-list-item-content>
                <v-list-item-title><span class="userMenu_titles">{{$t('menuList_my_study_rooms')}}</span></v-list-item-title>
              </v-list-item-content>
            </v-list-item>
          </template>
        </template>

        <!-- Not Logged User -->
        <template v-if="!isLoggedIn">
            <v-list-item 
                v-for="link in satelliteLinksNotLogged" 
                :key="link.title" 
                :href="link.url" 
                target="_blank"
                link 
                exact
                sel="menu_row" 
            >
            <v-list-item-action>
                <v-icon class="userMenu_icons">{{link.icon}}</v-icon>
            </v-list-item-action>
            <v-list-item-content>
                <div class="v-list__tile__title subheading userMenu_titles">{{link.title}}</div>
            </v-list-item-content>
          </v-list-item>
        </template>

        <component
          :is="menuListUserType"
          :satelliteLinksTeacher="satelliteLinksTeacher"
          :satelliteLinksStudent="satelliteLinksStudent"
        ></component>

        <!-- Logout -->
        <v-list-item @click="logout" v-if="isLoggedIn" sel="menu_row">
          <v-list-item-action>
            <v-icon class="userMenu_icons">sbf-logout</v-icon>
          </v-list-item-action>
          <v-list-item-content>
            <v-list-item-title class="subheading userMenu_titles">{{$t('menuList_logout')}}</v-list-item-title>
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
import { LanguageChange } from "../../../../services/language/languageService";
import satelliteService from '../../../../services/satelliteService';

import intercomSettings from '../../../../services/intercomService';
import logoComponent from '../../../app/logo/logo.vue';
import menuListStudent from './menuListStudent.vue';
import menuListTeacher from './menuListTeacher.vue';

export default {
  components: {
    logoComponent,
    menuListStudent,
    menuListTeacher,
  },
  data() {
    return {
      component: '',
      menuListComponent: {
        teacher: 'menuListTeacher',
        student: 'menuListStudent'
      },
      languagesLocales,
      languageChoisesAval: [],
      satelliteLinksNotLogged: [
        {
          title: this.$t('menuList_help'),
          icon: 'sbf-help',
          url: satelliteService.getSatelliteUrlByName('faq')
        },
        {
          title: this.$t('menuList_about_spitball'), 
          icon: 'sbf-about',
          url: satelliteService.getSatelliteUrlByName('about')
        },
        {
          title: this.$t('menuList_terms_of_service'),
          icon: 'sbf-terms',
          url: satelliteService.getSatelliteUrlByName('terms') 
        },
        {
          title: this.$t('menuList_privacy_policy'),
          icon: 'sbf-privacy',
          url: satelliteService.getSatelliteUrlByName('privacy') 
        },
      ],
      satelliteLinksTeacher:[
        {
          title: this.$t('menuList_help'),
          icon: 'sbf-help',
          url: satelliteService.getSatelliteUrlByName('faq')
        },
        {
          title: this.$t('menuList_about_spitball'), 
          icon: 'sbf-about',
          url: satelliteService.getSatelliteUrlByName('about')
        },
        {
          title: this.$t('menuList_terms_of_service'),
          icon: 'sbf-terms',
          url: satelliteService.getSatelliteUrlByName('terms') 
        },
        {
          title: this.$t('menuList_privacy_policy'),
          icon: 'sbf-privacy',
          url: satelliteService.getSatelliteUrlByName('privacy') 
        },
        {
          title: this.$t('menuList_referral_spitball'),
          icon: 'sbf-user',
          action: this.openReferralDialog
        },
        {
          title: this.$t('menuList_feedback'),
          icon: 'sbf-feedbackNew',
          action: this.startIntercom 
        },
      ],
      satelliteLinksStudent: [
        {
          title: this.$t('menuList_account_setting'),
          icon: 'sbf-settings',
          url: 'profile'
        },
        {
          title: this.$t('menuList_my_purchases'),
          icon: 'sbf-cart',
          url: 'myPurchases'
        },
        {
          title: this.$t('menuList_my_studyroom'),
          icon: 'sbf-studyroom-icon',
          url: 'myStudyRooms'
        },
        {
          title: this.$t('menuList_my_sales'),
          icon: 'sbf-my-sales',
          url: 'mySales',
        },
      ]
    };
  },
  computed: {
    ...mapGetters(['accountUser', 'isFrymo', 'getUserLoggedInStatus', 'getIsStudent', 'getIsTeacher']),
    
    menuListUserType() {
      let userType = this.user?.userType || '';
      return this.menuListComponent[userType.toLowerCase()]
    },
    isMobile() {
      return this.$vuetify.breakpoint.smAndDown;
    },
    user() {
      return { ...this.accountUser };
    },
    isLoggedIn() {
      return this.getUserLoggedInStatus;
    },
    showChangeLanguage() {
      return global.country === 'IL';
    }
  },
  methods: {           
    ...mapActions(['updateReferralDialog', 'logout']),

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
    },
    openLoginDialog() {
      this.$store.commit('setComponent', 'login');
      this.$root.$emit('closeDrawer')
    },
    openRegisterTypeDialog() {
      this.$store.commit('setComponent', 'registerType');
      this.$root.$emit('closeDrawer')
    }
  },
  created() {
    let currentLocHTML = document.documentElement.lang;
    this.languageChoisesAval = languagesLocales.filter(lan => {
      return lan.locale !== currentLocHTML;
    });
  }
};
</script>

<style src="./menuList.less" lang="less"/>