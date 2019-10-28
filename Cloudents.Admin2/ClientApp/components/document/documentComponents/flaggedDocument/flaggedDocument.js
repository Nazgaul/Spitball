import flaggedDocumentService from './flaggedDocumentService.js'


    export default {
        data() {
            return {
                infoSuccess: '',
                infoError: '',
                documentsList: {
                    type: Array,
                    default: []
                },
                arrayOfIds: [],
                proccessedDocuments: [],
                bottomNav: 'refresh'

            };
        },
        methods: {
            markAsProccesed(arrIds) {
                for (let i = 0; i < arrIds.length; i++) {
                    this.proccessedDocuments.push(arrIds[i]);
                }
                return this.proccessedDocuments;
            },

            getDocumentsList() {
                let self = this;
                //clear proccessed array
                self.proccessedDocuments = [];
                self.arrayOfIds = [];
                flaggedDocumentService.getDocuments()
                    .then(resp => {
                        self.documentsList = resp;
                        self.arrayOfIds = self.documentsList.map(item => {
                            return item.id;
                        });
                    },
                        (error) => {
                            self.$toaster.error('Something went wrong');
                            console.log('component accept error', error);
                        });

            },
            deleteDocument(document) {
                let singleIdArr = [];
                flaggedDocumentService.deleteDocument(document.id)
                    .then(() => {
                        this.$toaster.success(`Document ${document.id} successfully deleted`);
                        singleIdArr.push(document.id);
                        // receives arr with id :: [12]
                        this.markAsProccesed(singleIdArr);
                    },
                        (error) => {
                            this.$toaster.error('Something went wrong');
                            console.log('component accept error', error);
                        });
            },
            //always one el array
            unflagSingleDocument(document) {
                let arrSingleId = [];
                arrSingleId.push(document.id);
                flaggedDocumentService.unflagDocument(arrSingleId)
                    .then(resp => {
                        this.$toaster.success(`Document ${arrSingleId} approved`);
                        console.log('docs!', resp);
                        this.markAsProccesed(arrSingleId);
                    },
                        (error) => {
                            this.$toaster.error('Something went wrong');
                            console.log('component accept error', error);
                        });
            },
            unflagDocuments() {
                let arrIds;
                //filter already deleted or approved, in order to prevent second processing attempt on server
                arrIds = this.arrayOfIds.filter((singleId) => {
                    return !this.proccessedDocuments.includes(singleId);
                });
                flaggedDocumentService.unflagDocument(arrIds)
                    .then(resp => {
                        this.$toaster.success(`All Documents ${arrIds} approved`);
                        this.getDocumentsList();
                        console.log('docs!', resp);
                    },
                        (error) => {
                            this.$toaster.error('Something went wrong');
                            console.log('component accept error', error);
                        });
            }
        },
        computed: {},
        created() {
            this.getDocumentsList();
        }
    }