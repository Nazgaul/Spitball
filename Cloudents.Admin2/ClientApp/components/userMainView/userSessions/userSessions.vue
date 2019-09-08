<template>
<div>
    <v-container fluid grid-list-sm>
        <v-layout row wrap>
            <v-flex xs12 v-for="(session, index) in userSessions" :key="index">
                <session-item :session="session"></session-item>
            </v-flex>
        </v-layout>
    </v-container>
  </div>
</template>

<script>
    import sessionItem from '../helpers/sessionItem.vue';
    import { mapGetters, mapActions } from 'vuex';

    export default {
        name: "userSessions",
        components: {sessionItem},
        props: {
            userId: {},
        },
        data() {
            return {
                loading: false
            }
        },
        computed: {
            ...mapGetters(["userSessions"]),
        },
        methods: {
            ...mapActions(["getUserSessions"]),
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