<template>
  <div>
    <v-toolbar color="indigo" dark>
      <v-toolbar-title>Documents List</v-toolbar-title>
    </v-toolbar>
    <v-container>
      <v-layout wrap row>
        <v-flex xs6 v-for="(document, index) in documentsList" :key="document.id">
          <v-card class="elevation-2 ma-2">
            <v-img :src="document.preview ? document.preview: `${require('../../../../assets/img/document.png')}`" aspect-ratio="2.75" contain></v-img>
            <v-card-text>
              <div>
                <b>Id:</b>
                {{document.id}}
              </div>
              
              <div>
                <b>Flagged User Email:</b>
                {{document.flaggedUserEmail}}
              </div>
              <div>
                <b>Reason:</b>
                {{document.reason}}
              </div>
            </v-card-text>

            <v-card-actions>
              <v-btn
                slot="activator"
                flat
                @click="unflagSingleDocument(document)"
                :disabled="proccessedDocuments.includes(document.id)"
              >
                Unflag
                <v-icon>check</v-icon>
              </v-btn>
              <v-btn
                slot="activator"
                flat
                color="purple"
                :disabled="proccessedDocuments.includes(document.id)"
                @click="deleteDocument(document)"
              >
                Delete
                <v-icon>delete</v-icon>
              </v-btn>

              <!-- <v-btn
                slot="activator"
                flat
                color="red"
                :href="document.siteLink"
                target="_blank"
              >Download</v-btn>

              <v-btn flat color="red" v-bind:href="document.siteLink" target="_blank">Link</v-btn> -->
            </v-card-actions>
          </v-card>
        </v-flex>
      </v-layout>
    </v-container>
    <v-bottom-nav app shift :active.sync="bottomNav" :value="true" color="#3f51b5">
      <v-btn class="bottom-nav-btn" dark value="approve" @click="unflagDocuments()">
        <div class="btn-text">Unflag All</div>
        <v-icon>check</v-icon>
      </v-btn>
    </v-bottom-nav>
  </div>
</template>
<script src="./flaggedDocument.js"></script>

<style lang="less" scoped>
</style>