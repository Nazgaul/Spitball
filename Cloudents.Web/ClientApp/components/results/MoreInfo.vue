<template functional>
    <general-page :title="props.name">
        <v-container slot="data" fluid grid-list-xl>
            <v-layout row justify-center>
                <!--<v-flex xs7 offset-xs5><div class="white"></div></v-flex>-->

                <v-flex v-for="(act,index) in props.actions" :key="index"><v-btn>{{act.name}}</v-btn></v-flex>
            </v-layout>
        </v-container>
    </general-page>
</template>
<script>
    export default {}
</script>