<template>
    <div class="tutor-list">
        <v-layout align-center justify-center justify-space-between class="tutor-list-header" tag="header">
            <v-flex class="tutor-list-header-left">
                <router-link class="tutor-list-header-logo ml-1" to="/">
                    <LogoSvg></LogoSvg>
                </router-link>
                <v-icon @click="handleMenuToggle()" class="tutor-list-header-left-menu hidden-md-and-up">sbf-menu</v-icon>
            </v-flex>
            <v-flex class="tutor-list-header-right hidden-sm-and-down" >
              <button
                class="tutor-list-header-right-login mr-2"
                @click="$router.push('../signin')"
                v-language:inner="'tutorListLanding_topnav_btn_login'"
              >
              </button>
              <button
                class="tutor-list-header-right-signup mr-3"
                @click="$router.push('../register')"
                v-language:inner="'tutorListLanding_topnav_btn_signup'"
              >
              </button>
              <a @click="changeLanguage()">
                {{currLanguage !== languageChoisesAval.id? languageChoisesAval.title : ''}}
              </a>
            </v-flex>

            <v-navigation-drawer 
              temporary v-model="drawer" 
              light 
              :right="!isRtl"
              fixed app v-if="isMobileView"
              :class="isRtl ? 'hebrew-drawer' : ''"
              width="280">
                 <menu-list :isAuthUser="loggedIn"></menu-list>
            </v-navigation-drawer>

        </v-layout>
    </div>
</template>

<script>
import LogoSvg from "../../../wwwroot/Images/logo-spitball.svg";
import languagesLocales from "../../services/language/localeLanguage";
import { LanguageChange } from "../../services/language/languageService";
import menuList from "../helpers/menu-list/menu-list.vue";
import { mapGetters } from 'vuex';

export default {
  components: {
    LogoSvg,
    menuList
  },
  name: "TopNav",
  data() {
    return {
      languageChoisesAval: [],
      navToggle: false,
      drawer: false,
      isRtl: global.isRtl,
      currLanguage: document.documentElement.lang
    };
  },
  methods: {
    changeLanguage() {
      LanguageChange.setUserLanguage(this.languageChoisesAval.id).then(
        resp => {
          console.log("language responce success", resp);
          global.location.reload(true);
        },
        error => {
          console.log("language error error", error);
        }
      );
    },
    handleMenuToggle() {
      this.drawer = !this.drawer
    }
  },
  computed: {
    ...mapGetters([
        "accountUser"
    ]),
    isMobileView() {
      return this.$vuetify.breakpoint.width < 1024;
    },
    loggedIn() {
      return !!this.accountUser
    },
  },
  created() {
    let currentLocHTML = document.documentElement.lang;
    this.languageChoisesAval = languagesLocales.filter(lan => {
      return lan.locale !== currentLocHTML;
    })[0];
  }
};
</script>

<style lang="less">
@import "../../styles/mixin.less";
.tutor-list {
  background: #fff;
  padding: 2px 10px;
  .tutor-list-header {
    max-width: 1500px; 
    margin: 0 auto;
    .tutor-list-header-left {
      display: flex;
      @media (max-width: @screen-sm) {
          justify-content: space-between
      }
      .tutor-list-header-logo {
        svg {
          vertical-align: -webkit-baseline-middle;
          fill: #1B2441
        }
      }
      .tutor-list-header-left-menu {
        color: #000;
        font-size: 14px;
      }
    }
    .tutor-list-header-right {
      display: flex;
      justify-content: flex-end;
      align-items: center;

      button {
        text-align: center;
        border-radius: 4px;
        font-size: 14px;
        outline: none;
        &.tutor-list-header-right-signup {
        padding: 5px;
        background-color: #13374d;
        color: rgba(255, 255, 255, 0.87);
      }
      &.tutor-list-header-right-login {
        padding: 5px;
        border: solid 1px #13374d;
        color: #13374d;
        background-color: transparent;
      }
      }
      
      a {
        text-decoration: none;
        font-size: 16px;
        color: #13374D;
        font-weight: bold;
      }
    }
  }
}
</style>