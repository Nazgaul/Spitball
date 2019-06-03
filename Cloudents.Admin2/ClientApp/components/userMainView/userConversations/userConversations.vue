<template>
<div>
    <v-container fluid grid-list-sm>
      <v-layout row wrap>
        <v-flex xs12 v-for="(conversation, index) in userConversations" :key="index">
          <conversation-item :conversation="conversation"></conversation-item>
        </v-flex>
      </v-layout>
    </v-container>
  </div>
</template>

<script>
    import conversationItem from '../helpers/conversationItem.vue';
    import { mapGetters, mapActions } from 'vuex';

    export default {
        name: "userConversations",
        components: {conversationItem},
        data() {
            return {
               
            }
        },
        props: {
            userId: {}
        },
       
        computed: {
            ...mapGetters(["userConversations"]),
        },
        methods: {
            ...mapActions(["getUserConversations"]),
            
            getUserConversationsData() {
                
                let id = this.userId;
               
                this.getUserConversations({id});
                }
        },
        created() {
            this.getUserConversationsData();
        },

    }
</script>

<style scoped>

</style>