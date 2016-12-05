module app {

    class Gamification {
        badgesState = {
            levels: 'l',
            badges: 'b',
            community: 'c'
        }
        badgesTab = this.badgesState.levels;
        constructor() {
            console.log("badge");
        }

        changeBadgesTab(tab) {
            this.badgesTab = tab;
            switch (this.badgesTab) {
                case this.badgesState.levels:
                    break;
                case this.badgesState.badges:
                    break;
                case this.badgesState.community:
                    break;
            }
        }

        badges = [
            {
                name: 'Spitballer',
                image: '',
                progress: 100,
                condition: 'Register',
                points: 500
            },
            {
                name: 'Explorer',
                image: '',
                progress: 100,
                condition: 'Follow 3 Classes',
                points: 500
            },
            {
                name: 'Harambe',
                image: '',
                progress: 80,
                condition: 'The ways to get this badge are secret. Seek and you shall find',
                points: 500
            },
            {
                name: 'Influencer',
                image: '',
                progress: 80,
                condition: 'Spread the word. Share a class or post on Facebook',
                points: 500
            },
            {
                name: 'Explorer',
                image: '',
                progress: 80,
                condition: 'Create a new class or department',
                points: 500
            },
            {
                name: 'Quizzy Lizzy',
                image: '',
                progress: 80,
                condition: 'Create a quiz',
                points: 500
            },
            {
                name: 'Helpful Harry',
                image: '',
                progress: 80,
                condition: 'You have one job: upload 3 documents to your class',
                points: 500
            },
            {
                name: 'Kiss from Keanu',
                image: '',
                progress: 80,
                condition: 'Like three posts or docs that you really think are of great value to unlock this badge',
                points: 500
            },
            {
                name: 'Narrator',
                image: '',
                progress: 80,
                condition: 'Your voice echoes through! Comment 3 times to get this badge',
                points: 500
            },
            {
                name: 'Quite Likely',
                image: '',
                progress: 80,
                condition: 'Like 10 items on our site to get this badge',
                points: 500
            },
            {
                name: 'Sharing is Caring',
                image: '',
                progress: 80,
                condition: 'Share a post or item 5 times. Stellar!',
                points: 500
            },
            {
                name: 'Early Bird',
                image: '',
                progress: 80,
                condition: 'Create a new university to get this badge',
                points: 500
            },
            {
                name: 'Quiz Master',
                image: '',
                progress: 80,
                condition: 'Score a 100% on a quiz',
                points: 500
            },
            {
                name: 'Secret Sally',
                image: '',
                progress: 80,
                condition: 'Create a private study group',
                points: 500
            },
            {
                name: 'Chatterbox',
                image: '',
                progress: 80,
                condition: 'Use the chat feature to communicate with a classmate',
                points: 500
            },
            {
                name: 'Private I',
                image: '',
                progress: 80,
                condition: 'Join or create a private department',
                points: 500
            }
        ];
        communityUsers = [
            {
                name: "Irena Dorfman",
                image: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S100X100/f2338ed9-d5be-4d39-8fca-2896ef836ef6.jpg",
                points: 50800
            },
            {
                name: "user 2",
                image: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S100X100/f2338ed9-d5be-4d39-8fca-2896ef836ef6.jpg",
                points: 800
            },
            {
                name: "user 3",
                image: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S100X100/f2338ed9-d5be-4d39-8fca-2896ef836ef6.jpg",
                points: 800000
            },
            {
                name: "user 4",
                image: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S100X100/f2338ed9-d5be-4d39-8fca-2896ef836ef6.jpg",
                points: 800000
            },
            {
                name: "user 5",
                image: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S100X100/f2338ed9-d5be-4d39-8fca-2896ef836ef6.jpg",
                points: 800000
            },
            {
                name: "user 6",
                image: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S100X100/f2338ed9-d5be-4d39-8fca-2896ef836ef6.jpg",
                points: 800000
            }];
    }

    angular.module("app.user").controller("gamification", Gamification);
}