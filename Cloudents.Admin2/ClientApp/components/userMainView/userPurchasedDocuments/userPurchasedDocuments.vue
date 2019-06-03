<template>

  <div>
    <v-container fluid grid-list-sm>
      <v-layout row wrap>
        <v-flex xs12 v-for="(document, index) in userPurchasedDocuments" :key="index">
          <purchased-doc-item :document="document"></purchased-doc-item>
        </v-flex>
      </v-layout>
    </v-container>
  </div>
</template>

<script>
import { mapActions, mapGetters } from "vuex";
import purchasedDocItem from "../helpers/purchasedDocItem.vue";

export default {
  name: "userPurchasedDocuments",
  components: { purchasedDocItem },
  data() {
    return {
      scrollFunc: {
        page: 0,
        doingStuff: false
      }
    };
  },
  props: {
    userId: {},
    needScroll: {}
  },
  computed: {
    ...mapGetters(["userPurchasedDocuments"]),
  
  },
  watch: {
    needScroll(val, oldval) {
      if (val && val != oldval) {
        this.getUserPurchasedDocs();
      }
    }
  },
  methods: {
    ...mapActions(["getUserPurchasedDocuments"]),
    nextPage() {
      this.scrollFunc.page++;
    },
    getUserPurchasedDocs() {
      let self = this;
      let id = self.userId;
      if (this.scrollFunc.doingStuff) {
        return;
      }
      let page = this.scrollFunc.page;
      this.scrollFunc.doingStuff = true;
      self.getUserPurchasedDocuments({ id, page }).then(isComplete => {
        self.scrollFunc.doingStuff = !isComplete;
        self.nextPage();
      });
    }
  },

  created() {
    this.getUserPurchasedDocs();
  }
};
</script>

<style scoped>
</style>