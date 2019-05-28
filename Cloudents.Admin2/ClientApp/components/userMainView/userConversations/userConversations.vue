<template>
<v-container  class="item-wrap">
    <v-layout>
        <v-flex xs12>
            <conversation-item
                    :conversations="UserConversations"
            ></conversation-item>
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
    import conversationItem from '../helpers/conversationItem.vue';
    import { mapGetters, mapActions } from 'vuex';

    export default {
        name: "userConversations",
        components: {conversationItem},
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
                "UserConversations"
            ]),
        },
        methods: {
            ...mapActions([
                "getUserConversations",
            ]),
            getUserConversationsData() {
                let id = this.userId;
                let self = this;
                self.loading = true;
                self.getUserConversations({id}).then((isComplete) => {
                  
                    self.loading = false;

                });
            }
        },
        created() {
            this.getUserConversationsData();
        },

    }
</script>

<style scoped>

</style>