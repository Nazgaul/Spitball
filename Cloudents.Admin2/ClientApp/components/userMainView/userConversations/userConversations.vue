<template>
<v-container  class="item-wrap">
    <v-layout>
        <v-flex xs12>
            <conversation-item
                    :conversations="UserConversations"
            ></conversation-item>
        </v-flex>
    </v-layout>
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