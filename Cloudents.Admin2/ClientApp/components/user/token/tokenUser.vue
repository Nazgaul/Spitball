<template>
    <div class="user-tokens-container">
        <h1>Give Tokens To User</h1>
        <div class="user-inputs-container">
            <span>User Id: <input type="text" v-model="userId" placeholder="insert user id"/></span>
            <span>Tokens: <input type="text" v-model.number="tokens" placeholder="set amount of tokens to apply"/></span>
        </div>
    </div>
    
</template>

<script>
export default {
    data(){
        return {
            userId: null,
            tokens: null
        }
    }
}
</script>

<style lang="scss" scoped>
.user-tokens-container{
    .user-inputs-container{
        display:flex;
        flex-direction: column;
        justify-content: center;
    }
}

</style>
