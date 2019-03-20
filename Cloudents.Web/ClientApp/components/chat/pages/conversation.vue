<template>
    <div class="conversation-container">
        <v-layout align-center justify-start row v-for="conversation in 4">
            <v-flex ml-2 class="avatar-container"><user-avatar :user-name="'gaby'" :user-id="123"/></v-flex>
            <v-flex class="user-detail-container">
                <v-flex class="top-detail-container">
                    <span>name {{conversation}}</span>
                    <span>date {{conversation}}</span>
                </v-flex> 
                <v-flex>
                    <span>message {{conversation}}</span>
                </v-flex> 
            </v-flex>
        </v-layout>
    </div>
</template>

<script>
import UserAvatar from '../../helpers/UserAvatar/UserAvatar.vue';
export default {
    components:{
        UserAvatar
    }
}
</script>

<style lang="less">
.conversation-container{
    width:100%;
    .avatar-container{
        flex-grow:0;      
    }
    .user-detail-container{
        padding:12px;
        .top-detail-container{
            display:flex;
            justify-content: space-between;
        }
    }
}
</style>
