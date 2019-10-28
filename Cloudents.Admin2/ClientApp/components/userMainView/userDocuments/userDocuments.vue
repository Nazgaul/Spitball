<template>
  <!-- <v-container fluid>
        <v-layout>
            <v-flex xs12>
                <document-item
                        :filterVal="filterValue" :documents="UserDocuments"
                ></document-item>
            </v-flex>
        </v-layout>
  </v-container>-->
  <div>
  <v-container fluid grid-list-sm>
      <v-layout row wrap>
          <v-flex xs4 v-for="(document, index) in filteredDocuments" :key="index" >
            <document-item :document="document" :filterVal="filterValue" v-on:dialog="openDialog" >
            </document-item>
        </v-flex>
    </v-layout>
  </v-container>
    <v-dialog :max-width="'1280px'" v-model="dialog.showBigImageDialog">
                <v-card >
                    <v-toolbar color="primary">
                        <v-toolbar-title>Preview</v-toolbar-title>
                         <v-spacer></v-spacer>
                        <v-toolbar-items>
                            <v-btn icon >
                                 <v-icon @click="dialog.showBigImageDialog = false">close</v-icon>
                            </v-btn>
                        </v-toolbar-items>
                    </v-toolbar>
                    <img :src="dialog.img" alt="">
                </v-card>
        </v-dialog>
        </div>
</template>
<script>
import { mapActions, mapGetters } from "vuex";
import documentItem from "../helpers/documentItem.vue";

export default {
  name: "userDocuments",
  components: { documentItem },
  data() {
    return {
      scrollFunc: {
        page: 0,
        doingStuff: false
      },
      dialog: {
          img:'',
          showBigImageDialog:false
      }
    };
  },
  props: {
    userId: {},
    needScroll: {}
  },
  computed: {
    ...mapGetters(["userDocuments", "filterValue", 'getRequestLock']),
    filteredDocuments: function() {
         return  this.userDocuments.filter(f=> f.state === this.filterValue);
    }
  },
  watch: {
    needScroll(val, oldval) {
      if (val && val != oldval) {
        this.getUserDocumentsData();
      }
    }
  },
  methods: {
    ...mapActions(["getUserDocuments"]),
    nextPage() {
      this.scrollFunc.page++;
    },
    getUserDocumentsData() {
      let self = this;
      let id = self.userId;
      if (this.scrollFunc.doingStuff) {
        return;
      }
      let page = this.scrollFunc.page;
      this.scrollFunc.doingStuff = true;
      if(!this.getRequestLock) {
        self.getUserDocuments({ id, page }).then(isComplete => {
          self.scrollFunc.doingStuff = !isComplete;
          self.nextPage();
        });
      }
    },
    openDialog(img) {
        console.log(img);
        this.dialog.img = img;
        this.dialog.showBigImageDialog = true;
    }
  },
  created() {
    this.getUserDocumentsData();
  }
};
</script>

