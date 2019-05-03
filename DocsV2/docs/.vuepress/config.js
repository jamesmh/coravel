module.exports = {
    title: 'Coravel',
    description: 'Near-zero config .NET Core library that makes Task Scheduling, Caching, Queuing, Mailing, Event Broadcasting (and more) a breeze!',
    markdown: {
        toc: {
            includeLevel: [1,2]
        }
    },
    themeConfig: {
        sidebarDepth: 0,
        displayAllHeaders: true,
        nav: [{
            text: 'Home',
            link: '/'
        }],
        sidebar: [{
                title: 'Getting Started',
                collapsable: false,
                children: [
                    'Installation/'
                ]
            },
            {
                title: 'Core',
                collapsable: false,
                children: [
                    'Invocables/'
                ]
            },
            {
                title: 'Features',
                collapsable: false,
                children: [
                    'Scheduler/',
                    'Queuing/',
                    'Caching/',
                    'Events/',
                    'Mailing/'
                ]
            },
            {
                title: 'Extras',
                collapsable: false,
                children: [
                    'Cli/',
                    'Pro/'
                ]
            }                 
        ]
    }
}