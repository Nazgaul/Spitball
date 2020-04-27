<template>
<div>
    <v-card class="elevation-5">
          <v-img :src="document.preview ? document.preview: `${require('../../../assets/img/document.png')}`" @click="imageView(document.preview)" aspect-ratio="2.75" contain>
          </v-img>
          <v-card-title primary-title>
        {{document.name}}
        </v-card-title>
        <v-card-text>
            <div><b>Course:</b> {{document.course}}</div>
            <div><b>Upload Date:</b>  {{document.create | dateFromISO}}</div>
        </v-card-text>
        <v-card-actions>
            <v-btn flat v-if="!isOk && !isDeleted" @click="isFlagged ? unflagSingleDocument(document, index) : approveSingleDocument(document, index)"
                                   :disabled="proccessedDocuments.includes(document.id)">
                                    <v-icon>check</v-icon>
                                    Accept
            </v-btn>
            <v-btn  flat v-if="!isDeleted" :disabled="proccessedDocuments.includes(document.id)"  @click="deleteDocument(document, index)">
                 <v-icon>delete</v-icon>
                 Delete
            </v-btn>
            <v-btn v-if="!isDeleted"  flat :href="document.siteLink">
                <v-icon>link</v-icon>
                Download
            </v-btn>
            
                    
        </v-card-actions>
    </v-card>
    </div>
   
</template>

<script>
    import flaggedDocumentService from '../../document/documentComponents/flaggedDocument/flaggedDocumentService';
    import approveDeleteService from '../../document/documentComponents/approveDelete/approveDeleteService';
    import { mapActions } from 'vuex';

    export default {
        name: "documentItem",
        data() {
            return {
                proccessedDocuments: [],
            }
        },
        props: {
            document: {},
            filterVal: {
                type: String,
                required: false
            },

        },
        computed: {
            ...mapActions(['deleteDocumentItem']),
            isOk() {
                console.log(this.filterVal);
                return this.filterVal === 'ok'
            },
            isPending() {
                return this.filterVal === 'pending'
            },
            isFlagged() {
                return this.filterVal === 'flagged'
            },
            isDeleted() {
                return this.filterVal === 'deleted'
            }
        },
        methods: {
            isVisible(itemState) {
                return itemState.toLowerCase() === this.filterVal.toLowerCase();
            },
            imageView(src) {
                this.$emit("dialog",src);
            },
           
            markAsProccessed(arrIds) {
                for (let i = 0; i < arrIds.length; i++) {
                    this.proccessedDocuments.push(arrIds[i])
                }
                return this.proccessedDocuments
            },
            deleteDocument(document, index) {
                let singleIdArr = [];
                flaggedDocumentService.deleteDocument(document.id)
                    .then(resp => {
                            this.$toaster.success(`Document ${document.id} successfully deleted`);
                            singleIdArr.push(document.id);
                            // receives arr with id :: [12]
                            this.markAsProccessed(singleIdArr);
                            this.deleteDocumentItem(index);
                        },
                        (error) => {
                            this.$toaster.error('Something went wrong');
                            console.log('component accept error', error)
                        })
            },
            //always one el array
            unflagSingleDocument(document, index) {
                let arrSingleId = [];
                arrSingleId.push(document.id);
                flaggedDocumentService.unflagDocument(arrSingleId)
                    .then(resp => {
                            this.$toaster.success(`Document ${arrSingleId} approved`);
                            this.markAsProccessed(arrSingleId);
                            this.updateData(index)
                        },
                        (error) => {
                            this.$toaster.error('Something went wrong');
                            console.log('component accept error', error)
                        })
            },
            //always one el array
            approveSingleDocument(document, index) {
                let arrSingleId = [];
                arrSingleId.push(document.id);
                approveDeleteService.approveDocument(arrSingleId)
                    .then(resp => {
                            this.$toaster.success(`Document ${arrSingleId} approved`);
                            console.log('docs!', resp);
                            this.markAsProccessed(arrSingleId);
                            // this.deleteDocumentItem(index);
                        },
                        (error) => {
                            this.$toaster.error('Something went wrong');
                            console.log('component accept error', error)
                        })
            },

        }

    }
</script>