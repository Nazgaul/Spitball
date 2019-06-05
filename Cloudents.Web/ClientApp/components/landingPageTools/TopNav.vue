<template>
  <section style="background-color: white">
    <v-layout align-center justify-space-between class="home-header warraper" tag="header">
      <v-flex class="topnav-left-section">
        <router-link class="nav-logo-warraper" to="/">
          <LogoSvg></LogoSvg>
        </router-link>
        <router-link class="topnav-links"
                     v-for="(link, index) in links"
                     :to="`${link.url}`"
                     :key="index">
                     {{link.title}}
        </router-link>
      </v-flex>
      <v-flex class="topnav-btns">
        <button
          class="topnav-btns-login mr-2"
          @click="$router.push('../signin')"
          v-language:inner="'landingPage_topnav_btn_login'"
        >login</button>
        <button
          class="topnav-btns-signup mr-5"
          @click="$router.push('../register')"
          v-language:inner="'landingPage_topnav_btn_signup'"
        >Sign Up</button>
        <a
          class="nav-heb"
          @click="changeLanguage()"
        >{{currLanguage !== languageChoisesAval.id? languageChoisesAval.title : ''}}</a>
      </v-flex>
    </v-layout>
  </section>
</template>

<script>
import LogoSvg from "../assets/logo.svg";
import languagesLocales from "../../../services/language/localeLanguage";
import { LanguageChange } from "../../../services/language/languageService";

export default {
  components: {
    LogoSvg
  },
  name: "TopNav",
  data() {
    return {
      languageChoisesAval: [],
      navToggle: false,
      links: [
        // {title: 'Find a tutor', url: 'find-tutor'},
        { title: "How it works", url: "how-it-works" },
        { title: "Become a tutor", url: "become-tutor" }
      ],
      currLanguage: document.documentElement.lang
    };
  },
  methods: {
    navToogle(e) {
      let el = this.$refs.dropdownMenu;
      let target = e.target;
      if (el !== target && !el.contains(target)) {
        this.navToggle = false;
      }
    },
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
    }
  },
  created() {
    document.addEventListener("click", this.navToogle);
    let currentLocHTML = document.documentElement.lang;
    this.languageChoisesAval = languagesLocales.filter(lan => {
      return lan.locale !== currentLocHTML;
    })[0];
  },
  destroyed() {
    document.removeEventListener("click", this.navToogle);
  }
};
</script>

<style lang="less">
.home-header {
  margin: 0 auto;
  height: 60px;
  background-color: #ffffff;
}
.topnav-container-flex {
  display: flex;
  justify-content: space-between;
}
.topnav-right-section {
  display: flex;
}
.topnav-btns {
  display: flex;
  justify-content: flex-end;
  align-items: baseline;
}

.topnav-btns-signup {
  width: 121px;
  height: 41px;
  background-color: #13374d;
  font-size: 20px;
  color: rgba(255, 255, 255, 0.87);
}
.topnav-btns-login {
  width: 121px;
  height: 41px;
  border: solid 1px #13374d;
  color: #13374d;
  background-color: #fff;
}
button {
  text-align: center;
  border-radius: 4px;
  font-size: 20px;
  outline: none;
}

.topnav-links {
  font-size: 20px;
  color: #13374d;
  margin-right: 64px;
}
.topnav-left-section {
  display: flex;
}
a {
  text-decoration: none;
}
</style>