<template>
<v-container  class="item-wrap">
    <v-layout>
        <v-flex xs12>
            <session-item
                    :sessions="UserSessions"
            ></session-item>
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
    import sessionItem from '../helpers/sessionItem.vue';
    import { mapGetters, mapActions } from 'vuex';

    export default {
        name: "userSessions",
        components: {sessionItem},
        data() {
            return {
                loading: false,
            }
        },
        props: {
            userId: {

            },
        },
        computed: {
            ...mapGetters([
                "UserInfo",
                "UserSessions"
            ]),
        },
        methods: {
            ...mapActions([
                "getUserSessions",
            ]),
            getUserSessionsData() {
                let id = this.userId;
                let self = this;
                self.loading = true;
                self.getUserSessions({id}).then((isComplete) => {
                  
                    self.loading = false;

                });
            }
        },
        created() {
            this.getUserSessionsData();
        },

    }
</script>

<style scoped>

</style>