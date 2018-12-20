<template>
    <div class="container">
        <v-layout justify-center>
            <v-flex xs12 sm6>
                <v-toolbar color="indigo" dark>
                    <v-toolbar-title>Documents List</v-toolbar-title>
                </v-toolbar>
                <v-card>
                    <v-container fluid grid-list-md>
                        <v-layout row wrap>
                            <h3 v-if="!documentsList.length">No documents to display</h3>
                            <v-flex v-else v-for="(document, index) in documentsList" :key="document.id">
                                <v-card>
                                    <v-img :class="[ 'document-preview', proccessedDocuments.includes(document.id) ? 'blured' : '']"
                                           :src="document.preview"
                                           height="200px">
                                        <v-container fill-height
                                                     fluid
                                                     pa-2>
                                            <v-layout fill-height>
                                                <v-flex xs12 align-end flexbox>
                                                    <span class="headline" v-text="document.id"></span>
                                                </v-flex>
                                                <v-flex xs12 align-end flexbox>
                                                    <span class="headline" v-text="document.flaggedUserEmail"></span>
                                                </v-flex>
                                                <v-flex xs12 align-end flexbox>
                                                    <span class="headline" v-text="document.reason"></span>
                                                </v-flex>
                                            </v-layout>
                                        </v-container>
                                    </v-img>
                                    <v-card-actions>
                                        <v-btn flat
                                               @click="unflagSingleDocument(document)"
                                               :disabled="proccessedDocuments.includes(document.id)">
                                            Unflag
                                            <v-icon>check</v-icon>
                                        </v-btn>
                                        <v-btn flat color="purple"
                                               :disabled="proccessedDocuments.includes(document.id)"
                                               @click="deleteDocument(document)">
                                            Delete
                                            <v-icon>delete</v-icon>
                                        </v-btn>

                                        <!-- <v-btn flat color="red" v-bind:href="document.siteLink" target="_blank">
                                            Link
                                        </v-btn> -->
                                        <v-spacer></v-spacer>
                                    </v-card-actions>
                                </v-card>
                            </v-flex>
                        </v-layout>
                    </v-container>
                </v-card>
            </v-flex>
        </v-layout>
        <v-bottom-nav app
                      shift
                      :active.sync="bottomNav"
                      :value="true"
                      color="#3f51b5">
            <!-- <v-btn class="bottom-nav-btn" dark  value="refresh" @click="getDocumentsList()">
                <span class="btn-text">Get another 20</span>
                <v-icon>refresh</v-icon>
            </v-btn> -->
            <v-btn class="bottom-nav-btn" dark value="approve" @click="unflagDocuments()">
                <span class="btn-text">Unflag All</span>
                <v-icon>check</v-icon>
            </v-btn>
        </v-bottom-nav>
    </div>
</template>
<script src="./flaggedDocument.js"></script>

<style lang="scss" scoped>
    .container {
        .document-preview

    {
        &.blured

    {
        -webkit-filter: grayscale(100%); /* Safari 6.0 - 9.0 */
        filter: grayscale(100%);
    }

    }

    .bottom-nav-btn {
        opacity: 1 !important;
        .btn-text

    {
        color: #ffffff !important;
        font-size: 16px;
        font-weight: 400;
    }

    }
    }
</style>