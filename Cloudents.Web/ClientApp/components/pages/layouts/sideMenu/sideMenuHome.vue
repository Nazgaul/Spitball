<template>
   <v-list-group class="sideMenu_group" :prepend-icon="'sbf-home-sideMenu'" no-action :append-icon="''" active-class="''">
      <template v-slot:activator>
         <v-list-item class="sideMenu_list">
            <v-list-item-content>
            <v-list-item-title>
               <span class="sideMenu_list_title" v-language:inner="'schoolBlock_home'"/>
            </v-list-item-title>
            </v-list-item-content>
         </v-list-item>
      </template>
   </v-list-group>
</template>
