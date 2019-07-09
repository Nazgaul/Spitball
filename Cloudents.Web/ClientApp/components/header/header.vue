<template>
        <main-header v-if="!hideHeaderMobile" ref="mainHeader" :userText="userText" 
                     :submitRoute="submitRoute" :currentSelection="currentSelection">
        </main-header>
</template>
<script src="./header.js"></script>


