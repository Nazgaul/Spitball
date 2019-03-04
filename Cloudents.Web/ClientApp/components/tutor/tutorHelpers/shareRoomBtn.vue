<template>
    <v-btn @click="copyUrl" :color="isCopied ? 'success' : 'primary'">{{isCopied ? 'Copied' : 'Share'}}</v-btn>
</template>

<script>
    export default {
        name: "shareRoomBtn",
        data() {
            return {
                isCopied: false
            }
        },
        methods: {
            copyUrl(){
                let url = global.location.href;
                    let self = this;
                    self.$copyText(url).then((e) => {
                        self.isCopied = true;
                    }, (e) => {
                    })
            }
        },
    }
</script>

<style scoped>

</style>