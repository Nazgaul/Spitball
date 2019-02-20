<template>
    <v-container  class="item-wrap">
        <v-layout>
            <v-flex xs12>
                <document-item
                        :filterVal="filterValue" :documents="UserDocuments"
                ></document-item>
            </v-flex>
        </v-layout>
        <v-progress-circular
                style="position: absolute; top: 300px; left: auto; right: auto;"
                :size="150"
                class="loading-spinner"
                color="#00bcd4"
                v-show="loading"
                indeterminate
        >
            <span>Loading...</span>
        </v-progress-circular>
    </v-container>
</template>

<script>
    import { mapActions, mapGetters } from 'vuex';
    import documentItem from '../helpers/documentItem.vue';

    export default {
        name: "userDocuments",
        components: {documentItem},
        data() {
            return {
                loading: false,
                scrollFunc: {
                    page: 0,
                    getData: this.getUserDocumentsData,
                    scrollLock: false,
                    wasCalled: false
                },

                searchQuery: ''
            }
        },
        props: {
            userId: {
            },
        },
        computed: {
            ...mapGetters([
                "UserInfo",
                "UserDocuments",
                "filterValue"
            ]),
        },
        methods: {
            ...mapActions([
                "getUserDocuments",
            ]),
            nextPage() {
                this.scrollFunc.page++
            },
            getUserDocumentsData() {
                let self = this;
                let id = self.userId;
                let page = this.scrollFunc.page;
                this.scrollFunc.wasCalled = true;
                this.loading = true;
                self.getUserDocuments({id, page}).then((isComplete) => {
                    self.nextPage();
                    if(!isComplete){
                        self.scrollFunc.scrollLock = false;
                    }else{
                        self.scrollFunc.scrollLock  = true;
                    }
                    self.loading = false;

                });
            },
            handleScroll(event) {
                let offset = 2000;
                if (event.target.scrollHeight - offset < event.target.scrollTop) {
                    if (!this.scrollFunc.scrollLock) {
                        this.scrollFunc.scrollLock = true;
                        this.scrollFunc.getData(this.userId, this.scrollFunc.page )
                    }
                }
            },
            attachToScroll(){
                let containerElm = document.querySelector('.item-wrap');
                containerElm.addEventListener('scroll', this.handleScroll)
            }
        },
        created() {
            this.getUserDocumentsData();
            console.log('hello created' + this.userId);
            this.$nextTick(function () {
                this.attachToScroll();
            })
        },
        beforeDestroy() {
            let containerElm = document.querySelector('.item-wrap');
            if (!containerElm) return;
            containerElm.removeEventListener('scroll', this.handleScroll);
        }

    }
</script>

<style scoped>

</style>