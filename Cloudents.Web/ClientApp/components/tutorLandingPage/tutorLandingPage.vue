<template>
  <v-container class="tutor-landing-page-container">
    <v-layout
      :class="`${isMobile ? 'pt-2 pb-5' : 'pt-1 pb-3'}`"
      px-4
      class="tutor-landing-page-header"
      align-center
      justify-center
      column
    >
      <v-flex pt-4 pb-3>
        <h1 v-language:inner="'tutorListLanding_header_get_lesson'"></h1>
      </v-flex>
      <v-flex pb-4>
        <h2 v-language:inner="'tutorListLanding_header_find_tutors'"></h2>
      </v-flex>
      <v-flex :class="{'pb-4': !isMobile}">
        <h3>
          <span v-language:inner="'tutorListLanding_rates'"></span>&nbsp;
          <v-icon v-for="n in 5" :key="n" class="tutor-landing-page-star">sbf-star-rating-full</v-icon>&nbsp;
          <span v-language:inner="'tutorListLanding_reviews'"></span>
        </h3>
      </v-flex>
    </v-layout>
    <v-layout class="tutor-landing-page-search" align-center justify-center>
      <div class="tutor-search-container">
        <tutor-search-component></tutor-search-component>
      </div>
    </v-layout>
    <scroll-list
      :scrollFunc="scrollFunc"
      :isLoading="scrollBehaviour.isLoading"
      :isComplete="scrollBehaviour.isComplete"
      class="layout column tutor-landing-page-body"
    >
      <v-flex class="tutor-landing-page-empty-state">
        <empty-state-card
          v-if="items.length === 0 && query.term && showEmptyState"
          style="margin: 0 auto;"
          :userText="query.term"
        ></empty-state-card>
      </v-flex>
      <v-flex class="tutor-landing-card-container" v-for="(item, index) in items" :key="index">
        <tutor-result-card v-if="!isMobile" class="mb-3" :fromLandingPage="true" :tutorData="item"></tutor-result-card>
        <tutor-result-card-mobile v-else class="mb-2" :fromLandingPage="true" :tutorData="item"></tutor-result-card-mobile>
      </v-flex>
    </scroll-list>
    <v-layout align-center py-5 justify-space-around class="tutor-landing-status-row">
      <span class="hidden-xs-only">
        <span v-language:inner="'tutorListLanding_rates'"></span>&nbsp;
        <v-icon v-for="n in 5" :key="n" class="tutor-landing-page-star">sbf-star-rating-full</v-icon>&nbsp;
        <span v-language:inner="'tutorListLanding_reviews'"></span>
      </span>
      <span class="hidden-xs-only" v-language:inner="'tutorListLanding_courses'"></span>
      <span v-language:inner="'tutorListLanding_tutors'"></span>
    </v-layout>
    <v-layout></v-layout>
    <Footer></Footer>
  </v-container>
</template>

<script>
import tutorResultCard from "../results/tutorCards/tutorResultCard/tutorResultCard.vue";
import tutorResultCardMobile from "../results/tutorCards/tutorResultCardMobile/tutorResultCardMobile.vue";
import tutorSearchComponent from "./components/tutorSearchInput/tutorSearchInput.vue";
import tutorLandingPageService from "./tutorLandingPageService";
import emptyStateCard from "../results/emptyStateCard/emptyStateCard.vue";

export default {
  components: {
    tutorResultCard,
    tutorResultCardMobile,
    tutorSearchComponent,
    emptyStateCard
  },
  data() {
    return {
      items: [],
      query: {
        term: "",
        page: 0
      },
      showEmptyState: false,
      scrollBehaviour: {
        isLoading: false,
        isComplete: false,
        MAX_ITEMS: 25
      }
    };
  },
  computed: {
    isMobile() {
      return this.$vuetify.breakpoint.xsOnly;
    }
  },
  watch: {
    $route(val) {
      // console.log(val.query.term)
      this.query.term = val.query.term;
      this.query.page = 0;
      this.updateList();
    }
  },
  methods: {
    updateList() {
      this.showEmptyState = false;
      tutorLandingPageService.getTutorList(this.query).then(data => {
        if (data.length < this.scrollBehaviour.MAX_ITEMS) {
          this.scrollBehaviour.isComplete = true;
        }
        if (this.query.page > 0) {
          this.items = this.items.concat(data);
        } else {
          this.items = data;
        }
        this.showEmptyState = true;
        this.scrollBehaviour.isLoading = false;
      });
    },
    scrollFunc() {
      this.scrollBehaviour.isLoading = true;
      this.query.page = this.query.page + 1;
      this.updateList();
    }
  },
  created() {
    this.query.term =
      !!this.$route.query && !!this.$route.query.term
        ? this.$route.query.term
        : "";
    this.updateList();
  }
};
</script>

<style lang="less">
@import "../../styles/mixin.less";
.tutor-landing-page-container {
  max-width: 100%;
  padding: 0;
  margin: 0;
  .tutor-landing-page-star {
    color: #ffca54;
    font-size: 20px;
  }
  .tutor-landing-page-header {
    position: relative;
    background-color: #1b2441;
    h1 {
      color: #3dc2ba;
      font-size: 35px;
      font-weight: bold;
      @media (max-width: @screen-xs) {
        font-size: 32px;
      }
    }
    h2 {
      font-size: 25px;
      font-weight: bold;
      color: #ffffff;
      @media (max-width: @screen-xs) {
        font-size: 16px;
      }
    }
    h3 {
      font-size: 18px;
      font-weight: 600;
      color: rgba(255, 255, 255, 0.87);
      @media (max-width: @screen-xs) {
        font-size: 16px;
      }
    }
  }
  .tutor-landing-page-search {
    position: sticky;
    top: 30px;
    z-index: 99;
    .tutor-search-container {
      width: 90%;
      max-width: 740px;
      position: absolute;
      bottom: -26px;
      box-shadow: 0 7px 13px 0 rgba(0, 0, 0, 0.28);
      border-radius: 4px;
      @media (max-width: @screen-xs) {
        width: 100%;
      }
    }
  }

  .tutor-landing-page-body {
    margin-top: 15px;
    .tutor-landing-page-empty-state {
      margin: 35px 0;
      @media (max-width: @screen-xs) {
        margin: 45px 6px 25px;
      }
    }
    .tutor-landing-card-container {
      margin: 0 auto;
      padding: 0 15px;
      @media (max-width: @screen-xs) {
        margin: 0 8px;
        padding: 0;
      }
    }
  }
  .tutor-landing-status-row {
    background-color: #fff;
    padding: 0 290px;
    @media (max-width: @screen-md) {
      padding: 0;
    }

    span {
      font-size: 22px;
      font-weight: 600;
      color: rgba(0, 0, 0, 0.87);
    }
  }
}
</style>
