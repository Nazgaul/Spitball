<template>
     <v-dialog ref="person" v-model="showDialog" max-width="518" :fullscreen="isSearch" :content-class="isSearch?'dialog-choose':''" :overlay="!isSearch">
        <v-card v-show="!isSearch">
            <v-card-title class="">Personalize Results</v-card-title>
            <v-card-text>You can tailor the results to you by adding your school and classes.</v-card-text>
            <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn flat="flat" @click.native="showDialog = false">Not now</v-btn>
                <v-btn flat="flat" @click.native="$_personalize">Personalize</v-btn>
            </v-card-actions>
        </v-card>
         <search-item v-model="showDialog" v-show="isSearch" :type="type" :keep="keep" :isFirst="isfirst"></search-item>
    </v-dialog>
</template>
<style lang="less" src="./ResultPersonalize.less" scoped>
</style>
<script src="./ResultPersonalize.js">  
</script>